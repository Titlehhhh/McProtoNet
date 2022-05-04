namespace McProtoNet.API
{
    public static class McUtils
    {
        public static string ProtocolVersionToString(int version)
        {
            switch (version)
            {
                case 51: return "1.4.6";
                case 60: return "1.5.1";
                case 62: return "1.5.2";
                case 72: return "1.6";
                case 73: return "1.6.1";
                case 4: return "1.7.2";
                case 5: return "1.7.6";
                case 47: return "1.8";
                case 107: return "1.9";
                case 108: return "1.9.1";
                case 109: return "1.9.2";
                case 110: return "1.9.3";
                case 210: return "1.10";
                case 315: return "1.11";
                case 316: return "1.11.1";
                case 335: return "1.12";
                case 338: return "1.12.1";
                case 340: return "1.12.2";
                case 393: return "1.13";
                case 401: return "1.13.1";
                case 404: return "1.13.2";
                case 477: return "1.14";
                case 480: return "1.14.1";
                case 485: return "1.14.2";
                case 490: return "1.14.3";
                case 498: return "1.14.4";
                case 573: return "1.15";
                case 575: return "1.15.1";
                case 578: return "1.15.2";
                case 735: return "1.16";
                case 736: return "1.16.1";
                case 751: return "1.16.2";
                case 753: return "1.16.3";
                case 754: return "1.16.5";
                case 755: return "1.17";
                case 756: return "1.17.1";
                case 757: return "1.18.1";
                default: throw new InvalidOperationException("Неизвестная версия");
            }
        }

        public static int GetVarIntLength(this int val)
        {
            int amount = 0;
            do
            {
                val >>= 7;
                amount++;
            } while (val != 0);

            return amount;
        }
    }
}
