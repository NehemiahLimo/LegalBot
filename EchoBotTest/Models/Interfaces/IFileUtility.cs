using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBotTest.Models.Interfaces
{
    public interface IFileUtility
    {
        Task<string> ReadFromFile(string filePathSource);

        Task WriteToFile(string fileContents, string filePathSource);
    }
}
