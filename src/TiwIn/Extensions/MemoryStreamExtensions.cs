//-----------------------------------------------------------------------
// <copyright file="MemoryStreamExtensions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn.Extensions
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public static class MemoryStreamExtensions
    {
        public static async Task<string> ReadToEndAsync(this MemoryStream self, 
            Encoding? encoding = null, 
            bool detectEncodingFromByteOrderMarks = true, 
            int bufferSize = -1)
        {
            if (self is null) return null;
            var restorePosition = self.Position;
            try
            {
                self.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(self, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks, bufferSize);
                return await reader.ReadToEndAsync();
            }
            finally
            {
                self.Position = restorePosition;
            }
        }
    }
}
