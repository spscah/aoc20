using System;
using System.Collections.Generic;

namespace Handheld.Assembler.App
{
    class CodeRunner {
        IList<Instruction> _instructions;
        int _index; 
        IList<int> _history;

        int _accumulator; 

        bool _terminated;

        public int Accumulator { get { return _accumulator; } }
        public bool Terminated { get { return _terminated; } }

        public CodeRunner()
        {
            _instructions =  new List<Instruction>();
            Reset();
        }

        void Reset() {
            _accumulator = 0;
            _history = new List<int>();
            _index = 0;
            _terminated = false;
        }

        internal void AddAnInstruction(Instruction i)
        {
            _instructions.Add(i);
        }

        // Day 8 - part one
        internal int FirstCycle()
        {
            Reset();
            while(Step());
            return Accumulator;
        }

        // Day 8 - Part Two 
        internal int FindCorruption()
        {
            for(int i = 0; i < _instructions.Count; ++i)
            {
                if(_instructions[i].Operator != Operator.jmp && _instructions[i].Operator != Operator.nop)
                    continue;
                Instruction original = _instructions[i].Clone();
                Operator opposite = original.Operator == Operator.jmp ? Operator.nop : Operator.jmp;
                _instructions[i] = new Instruction(opposite, original.Offset);

                Reset();
                while(Step());
                _instructions[i] = original;
                if(Terminated){                    
                   break;
                }
            }
            return _accumulator;
        }

        bool Step()
        {
            if(_index == _instructions.Count)
            {
                _terminated = true;
                return false;
            }
            if(_history.Contains(_index))
                return false;
            Instruction currentInstruction = _instructions[_index];
            _history.Add(_index);
            switch(currentInstruction.Operator)
            {
                case(Operator.acc):
                    _accumulator += currentInstruction.Offset;
                    ++_index;
                    break;
                case(Operator.nop):
                    ++_index;
                    break;
                case(Operator.jmp):
                    _index += currentInstruction.Offset;
                    break;
            }
            return true; 
        }
    }
}
