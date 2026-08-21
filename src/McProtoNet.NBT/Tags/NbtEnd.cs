using System.Text;

namespace McProtoNet.NBT;

internal sealed class NbtEnd : NbtTag
{
    public override NbtTagType TagType => NbtTagType.End;

    internal override void ReadTag(NbtBinaryReader readStream)
    {
    }

    internal override void WriteTag(NbtBinaryWriter writeReader)
    {
        writeReader.Write(NbtTagType.End);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
    }

    public override object Clone()
    {
        return this;
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
    }
}