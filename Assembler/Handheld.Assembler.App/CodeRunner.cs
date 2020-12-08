using System;
using System.Collections.Generic;

namespace Handheld.Assembler.App
{
    class CodeRunner {
        #region member data 
        IList<Instruction> _instructions;
        int _index; 
        // history is the list of indices run - possibly only useful for day 8 
        IList<int> _history;

        int _accumulator; 

        bool _terminated;
        #endregion
        
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

        #region  day8
        // Day 8 - part one
        internal int FirstCycle()
        {
            Reset();
            while(Step());
            return _accumulator;
        }

        // Day 8 - Part Two 
        internal int FindCorruption()
        {
            _terminated = false;
            for(int i = 0; i < _instructions.Count; ++i)
            {
                if(_instructions[i].Operator != Operator.jmp && _instructions[i].Operator != Operator.nop)
                    continue;
                Instruction original = _instructions[i].Clone();
                _instructions[i] = new Instruction(original.Operator == Operator.jmp ? Operator.nop : Operator.jmp, original.Offset);

                Reset();
                while(Step());
                _instructions[i] = original;
                if(_terminated){                    
                   break;
                }
            }
            return _accumulator;
        }
        #endregion day8

        // Step will return true if we've stepped forward, false means we've stopped. If terminated correctly (i.e. at the index at the end of the code)
        // terminated will be set to true, too. 
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
