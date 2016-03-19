﻿using System;
namespace OpcodeTools
{
    public abstract class FormulasBase
    {
        // used in NetClient__ProcessMessage
        public abstract uint CalcCryptedFromOpcode(uint opcode);
        // for switch in NetClient__JAMClientDispatch
        public abstract uint CalcSpecialFromOpcode(uint opcode);
        // used in switch in NetClient__JAMClientConnectionDispatch
        public abstract uint CalcAuthFromOpcode(uint opcode);
        // used in NetClient__ProcessMessage
        protected abstract bool NormalCheck(uint opcode);
        // used in NetClient__ProcessMessage
        protected abstract bool SpecialCheck(uint opcode);
        // in function with "WORLD OF WARCRAFT CONNECTION - SERVER TO CLIENT" string
        protected abstract bool AuthCheck(uint opcode);
        // offset in NetClient__ProcessMessage
        protected virtual uint BaseOffset { get { return 1376; } }

        public bool IsAuthOpcode(uint opcode)
        {
            return AuthCheck(opcode);
        }

        public bool IsNormalOpcode(uint opcode)
        {
            return !IsSpecialOpcode(opcode) && !IsAuthOpcode(opcode) && NormalCheck(opcode);
        }
        
        public bool IsSpecialOpcode(uint opcode)
        {
            return !IsAuthOpcode(opcode) && SpecialCheck(opcode);
        }

        public uint CalcOffsetFromOpcode(uint opcode)
        {
            uint crypted = CalcCryptedFromOpcode(opcode);
            return (crypted * 4) + BaseOffset;
        }

        public uint CalcOpcodeFromSpecial(uint offset)
        {
            for (uint i = 1; i < 0xFFFF; ++i)
            {
                if (IsSpecialOpcode(i))
                {
                    if (CalcSpecialFromOpcode(i) == offset)
                        return i;
                }
            }
            return 0;
        }

        public uint CalcOpcodeFromOffset(uint offset)
        {
            for (uint i = 1; i < 0xFFFF; ++i)
            {
                if (IsNormalOpcode(i))
                {
                    if (CalcOffsetFromOpcode(i) == offset)
                        return i;
                }
            }
            return 0;
        }

        public uint CalcOpcodeFromAuth(uint auth)
        {
            for (uint i = 1; i < 0xFFFF; ++i)
            {
                if (IsAuthOpcode(i) &&
                    CalcAuthFromOpcode(i) == auth)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
