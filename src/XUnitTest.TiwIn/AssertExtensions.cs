//-----------------------------------------------------------------------
// <copyright file="AssertExtensions.cs" company="TiwIn">
// Copyright (c) TiwIn. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TiwIn
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Sdk;

    public static class AssertExtensions
    {
        public static async Task ThrowsAsync(Func<Exception, bool> assertionCallback, Func<Task> testCode)
        {
            if (assertionCallback == null) throw new ArgumentNullException(nameof(assertionCallback));
            if (testCode == null) throw new ArgumentNullException(nameof(testCode));
            try
            {
                await testCode.Invoke();
                throw new AssertActualExpectedException(new Exception(), null, "Expected an exception");
            }
            catch (AssertActualExpectedException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (!assertionCallback.Invoke(e))
                    Debug.WriteLine(e);
                Assert.True(assertionCallback.Invoke(e));
            }
        }
    }
}
