using System;
using ArgumentNullAnalyzer.Test.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgumentNullAnalyzer.Test
{
    [TestClass]
    public sealed class MarkupTextTests
    {
        [TestMethod]
        public void NoMarks_EmptyString()
        {
            var markupText = new MarkupText(string.Empty);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);
        }

        [TestMethod]
        public void NoMarks()
        {
            var text = "Foo";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: text, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);
        }

        [TestMethod]
        public void EmptyText_OnePosition()
        {
            var text = "$$";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 1, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[0]);
        }

        [TestMethod]
        public void EmptyText_TwoPositions()
        {
            var text = "$$$$";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 2, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[0]);
            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[1]);
        }

        [TestMethod]
        public void Position_Beginning()
        {
            var text = "$$Alpha Beta Gamma Delta";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 1, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[0]);
        }

        [TestMethod]
        public void Position_Middle()
        {
            var text = "Alpha Beta$$ Gamma Delta";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 1, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 10), actual: markupText.Positions[0]);
        }
        
        [TestMethod]
        public void Position_End()
        {
            var text = "Alpha Beta Gamma Delta$$";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 1, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 22), actual: markupText.Positions[0]);
        }

        [TestMethod]
        public void TwoPositions_Start()
        {
            var text = "$$$$Alpha Beta Gamma Delta";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 2, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[0]);
            Assert.AreEqual(expected: new LinePosition(0, 0), actual: markupText.Positions[1]);
        }

        [TestMethod]
        public void TwoPositions_Middle()
        {
            var text = "Alpha$$ Beta$$ Gamma Delta";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 2, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 5), actual: markupText.Positions[0]);
            Assert.AreEqual(expected: new LinePosition(0, 10), actual: markupText.Positions[1]);
        }

        [TestMethod]
        public void TwoPositions_End()
        {
            var text = "Alpha Beta Gamma Delta$$$$";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma Delta", actual: markupText.Text);
            Assert.AreEqual(expected: 2, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 0, actual: markupText.Spans.Length);

            Assert.AreEqual(expected: new LinePosition(0, 22), actual: markupText.Positions[0]);
            Assert.AreEqual(expected: new LinePosition(0, 22), actual: markupText.Positions[1]);
        }

        [TestMethod]
        public void EmptyText_OneSpan()
        {
            var text = "[||]";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 0));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }

        [TestMethod]
        public void EmptyText_TwoSpans()
        {
            var text = "[||][||]";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 2, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 0));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[1]);
        }

        [TestMethod]
        public void EmptyText_TwoSpans_Nested()
        {
            var text = "[|[||]|]";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: string.Empty, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 2, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 0));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[1]);
        }

        [TestMethod]
        public void Span_WholeText()
        {
            var text = "[|Alpha Beta Gamma|]";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma", actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 16));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }

        [TestMethod]
        public void Span_Beginning()
        {
            var text = "[|Alpha Beta|] Gamma";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma", actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 10));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }

        [TestMethod]
        public void Span_End()
        {
            var text = "Alpha [|Beta Gamma|]";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma", actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 6), new LinePosition(0, 16));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }

        [TestMethod]
        public void Span_Middle()
        {
            var text = "Alpha [|Beta|] Gamma";
            var markupText = new MarkupText(text);

            Assert.AreEqual(expected: "Alpha Beta Gamma", actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 6), new LinePosition(0, 10));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Span_MissingOpeningMark()
        {
            var text = "Alpha Beta|] Gamma";
            var markupText = new MarkupText(text);

            Assert.Fail("An exception should have been thrown");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Span_MissingClosingMark()
        {
            var text = "Alpha [|Beta Gamma";
            var markupText = new MarkupText(text);

            Assert.Fail("An exception should have been thrown");
        }

        [TestMethod]
        public void Span_Multiline()
        {
            var text = @"Alpha [|Beta
Gamma
Delta|]
Epsilon";
            var markupText = new MarkupText(text);

            var expectedText = @"Alpha Beta
Gamma
Delta
Epsilon";

            Assert.AreEqual(expected: expectedText, actual: markupText.Text);
            Assert.AreEqual(expected: 0, actual: markupText.Positions.Length);
            Assert.AreEqual(expected: 1, actual: markupText.Spans.Length);

            var expectedSpan = new LinePositionSpan(new LinePosition(0, 6), new LinePosition(2, 5));
            Assert.AreEqual(expected: expectedSpan, actual: markupText.Spans[0]);
        }
    }
}
