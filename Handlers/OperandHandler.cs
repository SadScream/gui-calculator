using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class OperandHandler
    {
        // класс для контроля операндов

        public Operand Left = new(true), Right = new();
        private bool rightIsActive = false;

        public Operand Active()
        {
            /*
             * возвращает активный операнд.
             * Активным является операнд, который мы вводим в данный момент
             * или ввода которого мы ожидаем. Например, при запуске активен всегда
             * левый, а после ввода операнда активным станет правый
             */

            if (rightIsActive)
            {
                return Right;
            }
            return Left;
        }

        public void SetLeftActive(bool waitForInputStart = true)
        {
            Left.SetWaiting(waitForInputStart);
            rightIsActive = false;
        }

        public void SetRightActive(bool waitForInputStart = true)
        {
            Right.SetWaiting(waitForInputStart);
            rightIsActive = true;
        }

        public bool RightIsActive()
        {
            return rightIsActive;
        }

        public bool LeftIsActive()
        {
            return !rightIsActive;
        }
    }
}
