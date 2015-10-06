using System;
using ArgumentNullAnalyzer.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace ArgumentNullAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = new MarkupText(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class [|TypeName|]
        {
        }
    }");
            var expected = new DiagnosticResult
            {
                Id = "ArgumentNullAnalyzer",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", test.Spans[0])
                }
            };

            VerifyCSharpDiagnostic(test.Text, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {
        }
    }";
            VerifyCSharpFix(test.Text, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ArgumentNullAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ArgumentNullAnalyzerAnalyzer();
        }
    }
}