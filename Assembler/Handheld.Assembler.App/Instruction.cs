using System;

namespace Handheld.Assembler.App
{
    class Instruction {
        readonly Operator _operator;
        readonly int _offset;

        public Operator Operator => _operator;
        public int Offset => _offset;

        public Instruction(Operator op, int off)
        {
            _operator = op;
            _offset = off;
        }

        internal Instruction Clone()
        {
            return new Instruction(_operator, _offset);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}{2}", _operator, _offset < 0 ? "" : "+", _offset);
        }
    }
}
