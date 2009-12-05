﻿using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSDefragLib.FileSystem.Ntfs
{
    public class NonResidentAttribute : Attribute
    {
        public UInt64 m_startingVcn;
        public UInt64 m_lastVcn;
        public UInt16 m_runArrayOffset;
        public Byte m_compressionUnit;
        public Byte[] m_alignmentOrReserved/*[5]*/;
        public UInt64 m_allocatedSize;
        public UInt64 m_dataSize;
        public UInt64 m_initializedSize;
        public UInt64 m_compressedSize;                  // Only when compressed

        private NonResidentAttribute()
        {
        }

        public static new NonResidentAttribute Parse(BinaryReader reader)
        {
            NonResidentAttribute a = new NonResidentAttribute();
            a.InternalParse(reader);
            a.m_startingVcn = reader.ReadUInt64();
            a.m_lastVcn = reader.ReadUInt64();
            a.m_runArrayOffset = reader.ReadUInt16();
            a.m_compressionUnit = reader.ReadByte();
            a.m_alignmentOrReserved = reader.ReadBytes(5);
            a.m_allocatedSize = reader.ReadUInt64();
            a.m_dataSize = reader.ReadUInt64();
            a.m_initializedSize = reader.ReadUInt64();
            a.m_compressedSize = reader.ReadUInt64();
            return a;
        }
    }
}