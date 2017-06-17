using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ephemera.Tiff.Infrastructure;

namespace Ephemera.Tiff.Fields
{
    [DebuggerDisplay("{Tag} ({Type})")]
    internal class VariableSizeTableTiffField : LongTiffField, ITiffFieldInternal
    {
        private readonly List<byte[]> tables = new List<byte[]>();

        public VariableSizeTableTiffField(ushort tag, TiffReader reader) : base(tag, reader) { }

        private VariableSizeTableTiffField(VariableSizeTableTiffField original) : base(original.TagNum, 0)
        {
            Offset = original.Offset;
            Values = new List<uint>(original.Values);
            tables.AddRange(original.tables.Select(x => x.ToArray()));
        }

        protected override void ReadTag(TiffReader reader)
        {
            base.ReadTag(reader);
            var pos = reader.BaseStream.Position;

            foreach (var offset in Values)
            {
                var bits = reader.ReadNBytes(offset, 16);
                var numValues = bits.Sum(x => x);
                var values = reader.ReadNBytes(offset + 16, (uint)numValues);
                var table = new byte[16 + numValues];
                bits.CopyTo(table, 0);
                values.CopyTo(table, 16);
                tables.Add(table);
            }

            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        public override void WriteData(BinaryWriter writer)
        {
            for (int i = 0; i < tables.Count; ++i)
            {
                writer.AlignToWordBoundary();
                Values[i] = (uint) writer.BaseStream.Position;
                writer.Write(tables[i]);
            }

            base.WriteData(writer);
        }

        ITiffFieldInternal ITiffFieldInternal.Clone()
        {
            return new VariableSizeTableTiffField(this);
        }
    }
}