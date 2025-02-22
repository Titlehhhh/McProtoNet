// Лицензировано .NET Foundation в соответствии с одним или несколькими соглашениями.
// .NET Foundation лицензирует этот файл вам по лицензии MIT.

using System.Buffers.Text;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Text.Unicode
{
    internal static unsafe partial class Utf16Utility
    {
        // Возвращает &inputBuffer[inputLength], если входной буфер действителен.
        /// <summary>
        /// Для входного буфера <paramref name="pInputBuffer"/> длиной char <paramref name="inputLength"/>,
        /// возвращает указатель на место, где появляются первые недопустимые данные в <paramref name="pInputBuffer"/>.
        /// </summary>
        /// <remarks>
        /// Возвращает указатель на конец <paramref name="pInputBuffer"/>, если буфер правильно сформирован.
        /// </remarks>
        public static char* GetPointerToFirstInvalidChar(char* pInputBuffer, int inputLength, out long utf8CodeUnitCountAdjustment, out int scalarCountAdjustment)
        {
            Debug.Assert(inputLength >= 0, "Входная длина не должна быть отрицательной.");
            Debug.Assert(pInputBuffer != null || inputLength == 0, "Входная длина должна быть равна нулю, если указатель входного буфера равен null.");

            // Сначала мы обработаем общий случай всех ASCII-символов. Если это может
            // потребить весь буфер, мы пропустим оставшуюся логику этого метода.

            int numAsciiCharsConsumedJustNow = (int)Ascii.GetIndexOfFirstNonAsciiChar(pInputBuffer, (uint)inputLength);
            Debug.Assert(0 <= numAsciiCharsConsumedJustNow && numAsciiCharsConsumedJustNow <= inputLength);

            pInputBuffer += (uint)numAsciiCharsConsumedJustNow;
            inputLength -= numAsciiCharsConsumedJustNow;

            if (inputLength == 0)
            {
                utf8CodeUnitCountAdjustment = 0;
                scalarCountAdjustment = 0;
                return pInputBuffer;
            }

            // Если мы дошли до сюда, это означает, что мы увидели некоторые не-ASCII данные, поэтому в наших
            // векторизованных путях кода ниже мы будем обрабатывать все не-суррогатные UTF-16
            // кодовые точки без ветвления. Мы будем ветвиться только если увидим суррогаты.
            //
            // Мы все еще оптимистично предполагаем, что данные в основном ASCII. Это означает, что
            // количество кодовых единиц UTF-8 и количество скаляров почти совпадает с количеством
            // кодовых единиц UTF-16. По мере прохождения входных данных и нахождения не-ASCII
            // символов, мы будем отслеживать эти "корректировки" исправлений. Чтобы получить
            // общее количество кодовых единиц UTF-8, необходимых для кодирования входных данных, добавьте
            // корректировку количества кодовых единиц UTF-8 к количеству увиденных кодовых единиц UTF-16.
            // Чтобы получить общее количество скаляров во входных данных,
            // добавьте корректировку количества скаляров к количеству увиденных кодовых единиц UTF-16.

            long tempUtf8CodeUnitCountAdjustment = 0;
            int tempScalarCountAdjustment = 0;
            char* pEndOfInputBuffer = pInputBuffer + (uint)inputLength;

            // Согласно https://github.com/dotnet/runtime/issues/41699, временно отключаем
            // пути кода с ARM64-интринсиками. Платформы ARM64 все еще могут использовать векторизованный
            // не-интринсифицированный блок 'else' ниже.

            if (/* (AdvSimd.Arm64.IsSupported && BitConverter.IsLittleEndian) || */ Sse2.IsSupported)
            {
                if (inputLength >= Vector128<ushort>.Count)
                {
                    Vector128<ushort> vector0080 = Vector128.Create((ushort)0x0080);
                    Vector128<ushort> vector7800 = Vector128.Create((ushort)0x7800);
                    Vector128<ushort> vectorA000 = Vector128.Create((ushort)0xA000);

                    char* pHighestAddressWhereCanReadOneVector = pEndOfInputBuffer - Vector128<ushort>.Count;
                    Debug.Assert(pHighestAddressWhereCanReadOneVector >= pInputBuffer);

                    do
                    {
                        Vector128<ushort> utf16Data = Vector128.Load((ushort*)pInputBuffer);

                        pInputBuffer += Vector128<ushort>.Count; // заранее увеличиваем это сейчас в подготовке к следующему циклу, при необходимости скорректируем позже

                        // Устанавливает бит 0x0080 каждого элемента в 'charIsNonAscii', если соответствующий
                        // вход был 0x0080 <= [значение]. (т.е., [значение] не ASCII.)

                        Vector128<ushort> charIsNonAscii = Vector128.Min(utf16Data, vector0080);

#if DEBUG
                        // Быстрая проверка, чтобы убедиться, что мы случайно не установили бит 0x8000 какого-либо элемента.
                        uint debugMask = charIsNonAscii.AsByte().ExtractMostSignificantBits();
                        Debug.Assert((debugMask & 0b_1010_1010_1010_1010) == 0, "Не должен был установить бит 0x8000 какого-либо элемента в 'charIsNonAscii'.");
#endif // DEBUG

                        // Устанавливает биты 0x8080 каждого элемента в 'charIsNonAscii', если соответствующий
                        // вход был 0x0800 <= [значение]. Это также обрабатывает отсутствующий диапазон несколькими строками выше.

                        // Поскольку 3-байтовые элементы имеют значение >= 0x0800, мы выполним насыщающее сложение 0x7800, чтобы
                        // получить все 3-байтовые элементы с установленными битами 0x8000. Насыщающее сложение не установит бит 0x8000
                        // для 1-байтовых или 2-байтовых элементов. Бит 0x0080 уже будет установлен для не-ASCII (2-байтовых
                        // и 3-байтовых) элементов.

                        Vector128<ushort> charIsThreeByteUtf8Encoded = Vector128.AddSaturate(utf16Data, vector7800);
                        uint mask = (charIsNonAscii | charIsThreeByteUtf8Encoded).AsByte().ExtractMostSignificantBits();

                        // Каждый четный бит маски будет 1 только если символ был >= 0x0080,
                        // и каждый нечетный бит маски будет 1 только если символ был >= 0x0800.
                        //
                        // Пример для UTF-16 входа "[ 0123 ] [ 1234 ] ...":
                        //
                        //            ,-- установлен если char[1] >= 0x0800
                        //            |   ,-- установлен если char[0] >= 0x0800
                        //            v   v
                        // mask = ... 1 1 0 1
                        //              ^   ^-- установлен если char[0] не ASCII
                        //              `-- установлен если char[1] не ASCII
                        //
                        // Это означает, что мы можем подсчитать количество установленных битов, и результат будет
                        // количеством *дополнительных* байтов UTF-8, которые требуются для каждой кодовой единицы UTF-16
                        // при расширении. Это дает неправильный подсчет для суррогатных кодовых единиц UTF-16
                        // (мы только что посчитали, что каждая отдельная кодовая единица расширяется до 3 байтов,
                        // но на самом деле правильно сформированная суррогатная пара расширяется до 4 байтов).
                        // Мы обработаем это через момент.
                        //
                        // Пока что вычислим popcnt, но отложим его. Мы добавим его к накопительному
                        // фактору корректировки UTF-8, как только определим, что в наших данных нет
                        // непарных суррогатов. (Непарные суррогаты сделали бы недействительным
                        // наш вычисленный результат, и нам пришлось бы его отбросить.)

                        nuint popcnt = (uint)BitOperations.PopCount(mask); // на x64 выполняем расширение нулями бесплатно

                        // Суррогаты нужно обрабатывать особым образом по двум причинам: (a) нам нужно
                        // учесть тот факт, что мы пересчитали в предыдущем сложении;
                        // и (b) они требуют отдельной проверки.
                        //
                        // Поскольку суррогатные кодовые точки находятся в [D800..DFFF], добавление {A000} к каждому элементу перемещает суррогатные
                        // кодовые точки в [7800..7FFF], что позволяет выполнить одно знаковое сравнение.

                        mask = Vector128.LessThan((utf16Data + vectorA000).AsInt16(), vector7800.AsInt16()).AsByte().ExtractMostSignificantBits();

                    FinishIteration:

                        // Примечание: биты маски установлены, когда соответствующий элемент НЕ является суррогатом.
                        // Мы инвертируем это перед входом в логику "проверки суррогатных пар" ниже.

                        if (mask == 0xFFFF)
                        {
                            // Поместим эту логику вверху, так как она предсказуемо выполняется (суррогатные пары редки).
                            // Либо мы не видели суррогатов, либо мы уже обработали их ниже.

                            tempUtf8CodeUnitCountAdjustment += (long)popcnt;
                            if (pInputBuffer > pHighestAddressWhereCanReadOneVector)
                            {
                                goto NonVectorizedLoop; // больше нельзя прочитать вектор данных
                            }
                        }
                        else
                        {
                            mask = ~mask;

                            // Присутствует как минимум одна кодовая единица UTF-16 суррогата.
                            // Поскольку мы выполнили операцию pmovmskb на результате 16-битного pcmpgtw,
                            // результирующие биты 'mask' будут встречаться парами:
                            // - 00 если соответствующий символ UTF-16 не был суррогатной кодовой единицей;
                            // - 11 если соответствующий символ UTF-16 был суррогатной кодовой единицей.
                            //
                            // Суррогатная кодовая единица UTF-16 высокого/низкого уровня имеет битовый шаблон [ 11011q## ######## ],
                            // где # - любой бит; q = 0 представляет высокий суррогат, а q = 1 представляет
                            // низкий суррогат. Сдвигая каждый суррогатный символ вправо на 3 бита, мы получаем
                            // [ 00011011 q####### ], что означает, что мы можем сразу использовать pmovmskb для
                            // определения, был ли данный символ высоким или низким суррогатом.
                            //
                            // Поэтому результирующие биты 'mask2' будут встречаться парами:
                            // - 00 если соответствующий символ UTF-16 был кодовой единицей высокого суррогата;
                            // - 01 если соответствующий символ UTF-16 был кодовой единицей низкого суррогата;
                            // - ## (мусор) если соответствующий символ UTF-16 не был суррогатной кодовой единицей.
                            //   Поскольку 'mask' уже имеет 00 в этих позициях (так как соответствующий символ
                            //   не был суррогатом), "mask AND mask2 == 00" верно для этих позиций.

                            uint mask2 = Vector128.ShiftRightLogical(utf16Data, 3).AsByte().ExtractMostSignificantBits();

                            // 'lowSurrogatesMask' имеет биты, встречающиеся парами:
                            // - 01 если соответствующий символ был символом низкого суррогата,
                            // - 00 если соответствующий символ был символом высокого суррогата или вообще не был суррогатом.

                            uint lowSurrogatesMask = mask2 & mask;

                            // 'highSurrogatesMask' имеет биты, встречающиеся парами:
                            // - 01 если соответствующий символ был символом высокого суррогата,
                            // - 00 если соответствующий символ был символом низкого суррогата или вообще не был суррогатом.

                            uint highSurrogatesMask = (mask2 ^ 0b_0101_0101_0101_0101u /* переключаем все четные биты 00 <-> 01 */) & mask;

                            Debug.Assert((highSurrogatesMask & lowSurrogatesMask) == 0,
                                "Символ не может быть одновременно и высоким, и низким суррогатом.");

                            Debug.Assert(((highSurrogatesMask | lowSurrogatesMask) & 0b_1010_1010_1010_1010u) == 0,
                                "Только четные биты (без нечетных) масок должны быть установлены.");

                            // Теперь проверим, что за каждым высоким суррогатом следует низкий суррогат, а каждый
                            // низкий суррогат следует за высоким суррогатом. Мы делаем исключение для случая, когда
                            // последний символ вектора является высоким суррогатом, так как мы не можем выполнить проверку
                            // на нем до следующей итерации цикла, когда мы надеемся получить соответствующий
                            // низкий суррогат.

                            highSurrogatesMask <<= 2;
                            if ((ushort)highSurrogatesMask != lowSurrogatesMask)
                            {
                                break; // ошибка: несоответствующая суррогатная пара; выход из векторизованной логики
                            }

                            if (highSurrogatesMask > ushort.MaxValue)
                            {
                                // В конце вектора был одиночный высокий суррогат.
                                // Мы скорректируем наши счетчики, чтобы не считать этот символ обработанным.

                                highSurrogatesMask = (ushort)highSurrogatesMask; // не позволяем одиночному высокому суррогату быть обработанным popcnt
                                popcnt -= 2; // биты '0xC000_0000' в исходной маске сдвигаются и отбрасываются, поэтому учитываем это здесь
                                pInputBuffer--; // не обрабатываем этот символ (указатель уже был увеличен в начале цикла)
                            }

                            // Если мы 64-битные, мы можем выполнить расширение нулями количества суррогатных пар
                            // бесплатно прямо сейчас, экономя шаг расширения несколькими строками ниже. Если мы 32-битные,
                            // преобразование в nuint сразу ниже является пустой операцией, и мы заплатим стоимость реального
                            // 64-битного расширения несколькими строками ниже.
                            nuint surrogatePairsCountNuint = (uint)BitOperations.PopCount(highSurrogatesMask);

                            // 2 символа UTF-16 становятся 1 скаляром Unicode

                            tempScalarCountAdjustment -= (int)surrogatePairsCountNuint;

                            // Поскольку каждая суррогатная кодовая единица была >= 0x0800, мы заранее предположили,
                            // что она будет закодирована как 3 кодовые единицы UTF-8, поэтому наше предыдущее вычисление popcnt
                            // предполагает, что пара кодируется как 6 кодовых единиц UTF-8. Поскольку каждая
                            // пара в реальности кодируется только как 4 кодовые единицы UTF-8, нам нужно
                            // выполнить эту корректировку сейчас.

                            if (IntPtr.Size == 8)
                            {
                                // Поскольку мы уже расширили нулями surrogatePairsCountNuint, мы можем напрямую
                                // выполнить sub + sub. Это эффективнее, чем shl + sub.
                                tempUtf8CodeUnitCountAdjustment -= (long)surrogatePairsCountNuint;
                                tempUtf8CodeUnitCountAdjustment -= (long)surrogatePairsCountNuint;
                            }
                            else
                            {
                                // Принимаем удар 64-битного расширения сейчас.
                                tempUtf8CodeUnitCountAdjustment -= 2 * (uint)surrogatePairsCountNuint;
                            }

                            mask = 0xFFFF; // отмечаем "нет суррогатов, требующих обработки"
                            goto FinishIteration; // прыжок назад для продолжения основного цикла
                        }
                    } while (true);

                    // Если мы достигли этой точки, мы увидели действительно недопустимые данные внутри цикла.
                    // Нужно отменить заранее выполненную корректировку "bump pInputBuffer", которая произошла в начале цикла.

                    pInputBuffer -= Vector128<ushort>.Count;
                }
            }
            else if (Vector128.IsHardwareAccelerated)
            {
                if (inputLength >= Vector128<ushort>.Count)
                {
                    Vector128<ushort> vector0080 = Vector128.Create<ushort>(0x0080);
                    Vector128<ushort> vector0400 = Vector128.Create<ushort>(0x0400);
                    Vector128<ushort> vector0800 = Vector128.Create<ushort>(0x0800);
                    Vector128<ushort> vectorD800 = Vector128.Create<ushort>(0xD800);

                    char* pHighestAddressWhereCanReadOneVector = pEndOfInputBuffer - Vector128<ushort>.Count;
                    Debug.Assert(pHighestAddressWhereCanReadOneVector >= pInputBuffer);

                    do
                    {
                        // Векторы 'twoOrMoreUtf8Bytes' и 'threeOrMoreUtf8Bytes' будут содержать
                        // элементы, значения которых равны 0xFFFF (-1 как знаковое слово), если соответствующая
                        // кодовая единица UTF-16 была >= 0x0080 и >= 0x0800 соответственно. Суммируя эти
                        // векторы, каждый элемент суммы будет содержать одно из трех значений:
                        //
                        // 0x0000 ( 0) = исходный символ был 0000..007F
                        // 0xFFFF (-1) = исходный символ был 0080..07FF
                        // 0xFFFE (-2) = исходный символ был 0800..FFFF
                        //
                        // Мы отрицаем их, чтобы получить значение 0..2 для каждого элемента, затем суммируем все
                        // элементы вместе, чтобы получить количество *дополнительных* кодовых единиц UTF-8,
                        // необходимых для представления этих данных UTF-16. Это похоже на шаг popcnt,
                        // выполняемый путем кода SSE2. Это приведет к избыточному подсчету суррогатов, но мы
                        // обработаем это вскоре.

                        Vector128<ushort> utf16Data = Vector128.Load((ushort*)pInputBuffer);
                        Vector128<ushort> twoOrMoreUtf8Bytes = Vector128.GreaterThanOrEqual(utf16Data, vector0080);
                        Vector128<ushort> threeOrMoreUtf8Bytes = Vector128.GreaterThanOrEqual(utf16Data, vector0800);
                        Vector128<nuint> sumVector = (Vector128<ushort>.Zero - twoOrMoreUtf8Bytes - threeOrMoreUtf8Bytes).AsNUInt();

                        // Мы попробуем суммировать по естественному слову (а не по 16-битному слову) за раз,
                        // что должно вдвое уменьшить количество операций, которые мы должны выполнить.

                        nuint popcnt = 0;
                        for (int i = 0; i < Vector128<nuint>.Count; i++)
                        {
                            popcnt += (nuint)sumVector[i];
                        }

                        uint popcnt32 = (uint)popcnt;
                        if (IntPtr.Size == 8)
                        {
                            popcnt32 += (uint)(popcnt >> 32);
                        }

                        // Как и в путях SSE4.1, вычисляем popcnt, но не включаем его, пока мы
                        // не узнаем, что во входных данных нет непарных суррогатов.

                        popcnt32 = (ushort)popcnt32 + (popcnt32 >> 16);

                        // Теперь проверяем на суррогаты.

                        utf16Data -= vectorD800;
                        Vector128<ushort> surrogateChars = Vector128.LessThan(utf16Data, vector0800);
                        if (surrogateChars != Vector128<ushort>.Zero)
                        {
                            // В векторе есть как минимум одна суррогатная (высокая или низкая) кодовая единица UTF-16.
                            // Мы построим дополнительные векторы: 'highSurrogateChars'
                            // и 'lowSurrogateChars', где элементы равны 0xFFFF, если исходная
                            // кодовая единица UTF-16 была высоким или низким суррогатом соответственно.

                            Vector128<ushort> highSurrogateChars = Vector128.LessThan(utf16Data, vector0400);
                            Vector128<ushort> lowSurrogateChars = Vector128.AndNot(surrogateChars, highSurrogateChars);

                            // Мы хотим убедиться, что за каждой кодовой единицей высокого суррогата следует
                            // кодовая единица низкого суррогата, а каждая кодовая единица низкого суррогата следует за
                            // кодовой единицей высокого суррогата. Поскольку у нас нет эквивалента pmovmskb
                            // или palignr, мы сделаем это в цикле. Мы не будем смотреть на
                            // самый последний элемент высокого суррогата, так как мы еще не знаем, будет ли
                            // следующий прочитанный вектор иметь элемент низкого суррогата.

                            if (lowSurrogateChars[0] != 0)
                            {
                                goto Error; // ошибка: начало буфера содержит отдельный символ низкого суррогата
                            }

                            ushort surrogatePairsCount = 0;
                            for (int i = 0; i < Vector128<ushort>.Count - 1; i++)
                            {
                                surrogatePairsCount -= highSurrogateChars[i]; // превращается в +1 или +0
                                if (highSurrogateChars[i] != lowSurrogateChars[i + 1])
                                {
                                    goto NonVectorizedLoop; // ошибка: несоответствующая суррогатная пара; выход из векторизованной логики
                                }
                            }

                            if (highSurrogateChars[Vector128<ushort>.Count - 1] != 0)
                            {
                                // В конце вектора был одиночный высокий суррогат.
                                // Мы скорректируем наши счетчики, чтобы не считать этот символ обработанным.

                                pInputBuffer--;
                                popcnt32 -= 2;
                            }

                            nint surrogatePairsCountNint = (nint)surrogatePairsCount; // расширение нулями до размера нативного целого

                            // 2 символа UTF-16 становятся 1 скаляром Unicode

                            tempScalarCountAdjustment -= (int)surrogatePairsCountNint;

                            // Поскольку каждая суррогатная кодовая единица была >= 0x0800, мы заранее предположили,
                            // что она будет закодирована как 3 кодовые единицы UTF-8. Каждая половина суррогата
                            // кодируется только как 2 кодовые единицы UTF-8 (всего 4 кодовые единицы UTF-8),
                            // поэтому мы скорректируем это сейчас.

                            tempUtf8CodeUnitCountAdjustment -= surrogatePairsCountNint;
                            tempUtf8CodeUnitCountAdjustment -= surrogatePairsCountNint;
                        }

                        tempUtf8CodeUnitCountAdjustment += popcnt32;
                        pInputBuffer += Vector128<ushort>.Count;
                    } while (pInputBuffer <= pHighestAddressWhereCanReadOneVector);
                }
            }

        NonVectorizedLoop:

            // Векторизация не поддерживается на нашей текущей платформе, или входные данные были слишком малы, чтобы получить
            // выгоду от векторизации, или мы увидели недопустимые данные UTF-16 в векторизованных путях кода и нам нужно
            // обработать оставшиеся допустимые символы перед тем, как сообщить об ошибке.

            for (; pInputBuffer < pEndOfInputBuffer; pInputBuffer++)
            {
                uint thisChar = pInputBuffer[0];
                if (thisChar <= 0x7F)
                {
                    continue;
                }

                // Увеличиваем корректировку на +1 для U+0080..U+07FF; на +2 для U+0800..U+FFFF.
                // Это оптимистично предполагает отсутствие суррогатов, которые мы обработаем вскоре.

                tempUtf8CodeUnitCountAdjustment += (thisChar + 0x0001_F800u) >> 16;

                if (!UnicodeUtility.IsSurrogateCodePoint(thisChar))
                {
                    continue;
                }

                // Found a surrogate char. Back out the adjustment we made above, then
                // try to consume the entire surrogate pair all at once. We won't bother
                // trying to interpret the surrogate pair as a scalar value; we'll only
                // validate that its bit pattern matches what's expected for a surrogate pair.

                tempUtf8CodeUnitCountAdjustment -= 2;

                if ((nuint)pEndOfInputBuffer - (nuint)pInputBuffer < sizeof(uint))
                {
                    goto Error; // input buffer too small to read a surrogate pair
                }

                thisChar = Unsafe.ReadUnaligned<uint>(pInputBuffer);
                if (((thisChar - (BitConverter.IsLittleEndian ? 0xDC00_D800u : 0xD800_DC00u)) & 0xFC00_FC00u) != 0)
                {
                    goto Error; // not a well-formed surrogate pair
                }

                tempScalarCountAdjustment--; // 2 UTF-16 code units -> 1 scalar
                tempUtf8CodeUnitCountAdjustment += 2; // 2 UTF-16 code units -> 4 UTF-8 code units

                pInputBuffer++; // consumed one extra char
            }

        Error:

            // Also used for normal return.

            utf8CodeUnitCountAdjustment = tempUtf8CodeUnitCountAdjustment;
            scalarCountAdjustment = tempScalarCountAdjustment;
            return pInputBuffer;
        }
    }
}
