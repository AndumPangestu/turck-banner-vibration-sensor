using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusWorkerService
{
    public static class ModbusHelper
    {
        public static double ConvertRegisterToDouble(ushort register, double divider = 1)
        {
            short signedValue = unchecked((short)register);
            return signedValue / divider;
        }
    }
}
