using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsCssJanus.Test
{
    [TestClass]
    public class CssJanusTest
    {
        private CssJanus cssJanus;

        [TestInitialize]
        public void Init()
        {
            cssJanus = new CssJanus();
        }

        [TestMethod]
        public void PreservesCommentsLeftLeftRight()
        {
            Assert.AreEqual(Flip(@"/* left /* right */right: 10px"), @"/* left /* right */left: 10px");
        }
        [TestMethod]
        public void PreservesCommentsLeftRightLeftRight()
        {
            Assert.AreEqual(Flip(@"/*left*//*left*/right: 10px"), @"/*left*//*left*/left: 10px");
        }
        [TestMethod]
        public void PreservesCommentsNewline()
        {
            Assert.AreEqual(Flip("/* Going right is cool */\n#test {right: 10px}"), "/* Going right is cool */\n#test {left: 10px}");
        }
        [TestMethod]
        public void PreservesCommentsNewlineBeforeAfter()
        {
            Assert.AreEqual(Flip("/* padding-right 1 2 3 4 */\n#test {right: 10px}\n/*right*/"), "/* padding-right 1 2 3 4 */\n#test {left: 10px}\n/*right*/");
        }
        [TestMethod]
        public void PreservesCommentsTwoLine()
        {
            Assert.AreEqual(Flip("/** Two line comment\n * left\n \\*/\n#test {right: 10px}"), "/** Two line comment\n * left\n \\*/\n#test {left: 10px}");
        }


        [TestMethod]
        public void FlipsAbsoluteOrRelativePositionValues()
        {
            Assert.AreEqual(Flip(@"right: 10px"), @"left: 10px");
        }


        [TestMethod]
        public void FlipsFourValueNotationPadding()
        {
            Assert.AreEqual(Flip(@"padding: .25em 0ex 0pt 15px"), @"padding: .25em 15px 0pt 0ex");
        }
        [TestMethod]
        public void FlipsFourValueNotationMargin()
        {
            Assert.AreEqual(Flip(@"margin: 1px 2px 3px -4px"), @"margin: 1px -4px 3px 2px");
        }
        [TestMethod]
        public void FlipsFourValueNotationPaddingMixed()
        {
            Assert.AreEqual(Flip(@"padding:0 0 .25em 15px"), @"padding:0 15px .25em 0");
        }
        [TestMethod]
        public void FlipsFourValueNotationPaddingMixedGradPercent()
        {
            Assert.AreEqual(Flip(@"padding: 1px 2% 3px 4.1grad"), @"padding: 1px 4.1grad 3px 2%");
        }
        [TestMethod]
        public void FlipsFourValueNotationPaddingWithAuto()
        {
            Assert.AreEqual(Flip(@"padding: 1px auto 3px 2px"), @"padding: 1px 2px 3px auto");
        }
        [TestMethod]
        public void FlipsFourValueNotationPaddingWithInherit()
        {
            Assert.AreEqual(Flip(@"padding: 1px auto 3px inherit"), @"padding: 1px inherit 3px auto");
        }
        [TestMethod]
        public void DoesNotFlipsSomethingThatResemblesFourValueNotation()
        {
            Assert.AreEqual(Flip(@"#settings td p strong"), @"#settings td p strong");
        }



        [TestMethod]
        public void FlipsThreeValueNotationMargin()
        {
            Assert.AreEqual(Flip(@"margin: 1em 0 .25em"), @"margin: 1em 0 .25em");
        }
        [TestMethod]
        public void FlipsThreeValueNotationNegativeMargin()
        {
            Assert.AreEqual(Flip(@"margin:-1.5em 0 -.75em"), @"margin:-1.5em 0 -.75em");
        }


        [TestMethod]
        public void FlipsTwoValueNotation()
        {
            Assert.AreEqual(Flip(@"padding: 1px 2px"), @"padding: 1px 2px");
        }
        [TestMethod]
        public void FlipsOneValueNotation()
        {
            Assert.AreEqual(Flip(@"padding: 1px"), @"padding: 1px");
        }




        [TestMethod]
        public void FlipsDirectionLtr()
        {
            Assert.AreEqual(Flip(@"direction: ltr"), @"direction: rtl");
        }
        [TestMethod]
        public void FlipsDirectionRtl()
        {
            Assert.AreEqual(Flip(@"direction: rtl"), @"direction: ltr");
        }
        [TestMethod]
        public void FlipsDirectionInsideSelector()
        {
            Assert.AreEqual(Flip(@"body { direction: rtl }"), @"body { direction: ltr }");
        }
        [TestMethod]
        public void FlipsDirectionInsideMultipleProperties()
        {
            Assert.AreEqual(Flip(@"body { padding: 10px; direction: rtl; }"), @"body { padding: 10px; direction: ltr; }");
        }
        [TestMethod]
        public void FlipsDirectionInsideMultipleSelector()
        {
            Assert.AreEqual(Flip(@"body { direction: rtl } .myClass { direction: ltr }"), @"body { direction: ltr } .myClass { direction: rtl }");
        }
        [TestMethod]
        public void FlipsDirectionInsideMultilineSelector()
        {
            Assert.AreEqual(Flip("body{\n direction: rtl\n}"), "body{\n direction: ltr\n}");
        }



        [TestMethod]
        public void FlipsRulesWithMoreThanOneHyphenRtl()
        {
            Assert.AreEqual(Flip("border-right-color: red"), "border-left-color: red");
        }
        [TestMethod]
        public void FlipsRulesWithMoreThanOneHyphenLtr()
        {
            Assert.AreEqual(Flip("border-left-color: red"), "border-right-color: red");
        }

        // This is for compatibility strength, in reality CSS has no properties that are
        // currently like this.
        [TestMethod]
        public void LeavesUnknownPropertyNamesAloneRight()
        {
            Assert.AreEqual(Flip("alright: 10px"), "alright: 10px");
        }
        [TestMethod]
        public void LeavesUnknownPropertyNamesAloneLeft()
        {
            Assert.AreEqual(Flip("alleft: 10px"), "alleft: 10px");
        }


        [TestMethod]
        public void FlipsFloatsRtl()
        {
            Assert.AreEqual(Flip("float: right"), "float: left");
        }
        [TestMethod]
        public void FlipsFloatsLtr()
        {
            Assert.AreEqual(Flip("float: left"), "float: right");
        }


        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffLeftFirst()
        {
            Assert.AreEqual(Flip("background: url(/foo/left-bar.png)", false, false), "background: url(/foo/left-bar.png)");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffLeftSecond()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-left.png)", false, false), "background: url(/foo/bar-left.png)");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffLtrSingleQuotes()
        {
            Assert.AreEqual(Flip("url('http://www.blogger.com/img/triangle_ltr.gif')", false, false), "url('http://www.blogger.com/img/triangle_ltr.gif')");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffLtrDoubleQuotes()
        {
            Assert.AreEqual(Flip("url(\"http://www.blogger.com/img/triangle_ltr.gif\")", false, false), "url(\"http://www.blogger.com/img/triangle_ltr.gif\")");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffSpaceBetweenQuotesAndParens()
        {
            Assert.AreEqual(Flip("url('http://www.blogger.com/img/triangle_ltr.gif'  )", false, false), "url('http://www.blogger.com/img/triangle_ltr.gif'  )");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffDotLeftNoQuotes()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar.left.png)", false, false), "background: url(/foo/bar.left.png)");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffDashRtlNoQuotes()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-rtl.png)", false, false), "background: url(/foo/bar-rtl.png)");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffDashRtlNoQuotesMultipleProperties()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-rtl.png); right: 10px", false, false), "background: url(/foo/bar-rtl.png); left: 10px");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffDashRtlNoQuotesMultiplePropertiesIncludingDirection()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-right.png); direction: ltr", false, false), "background: url(/foo/bar-right.png); direction: rtl");
        }
        [TestMethod]
        public void DoesNotFlipUrlsWhenFlagsAreOffDashRtlNoQuotesMultiplePropertiesIncludingDirectionAndXCoords()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-rtl_right.png);right:10px; direction: ltr", false, false), "background: url(/foo/bar-rtl_right.png);left:10px; direction: rtl");
        }



        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnRightSecond()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-right.png)", true, true), "background: url(/foo/bar-left.png)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnRightFirst()
        {
            Assert.AreEqual(Flip("background: url(/foo/right-bar.png)", true, true), "background: url(/foo/left-bar.png)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnRtlSingleQuotes()
        {
            Assert.AreEqual(Flip("url('http://www.blogger.com/img/triangle_rtl.gif')", true, true), "url('http://www.blogger.com/img/triangle_ltr.gif')");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnRtlDoubleQuotes()
        {
            Assert.AreEqual(Flip("url(\"http://www.blogger.com/img/triangle_rtl.gif\")", true, true), "url(\"http://www.blogger.com/img/triangle_ltr.gif\")");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnSpaceBetweenQuotesAndParens()
        {
            Assert.AreEqual(Flip("url('http://www.blogger.com/img/triangle_rtl.gif'	)", true, true), "url('http://www.blogger.com/img/triangle_ltr.gif'	)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnNoQuotesRight()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar.right.png)", true, true), "background: url(/foo/bar.left.png)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnFalseRightNoChange()
        {
            Assert.AreEqual(Flip("background: url(/foo/bright.png)", true, true), "background: url(/foo/bright.png)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnDashLtr()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-ltr.png)", true, true), "background: url(/foo/bar-rtl.png)");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnDashLtrMultipleProperties()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-ltr.png); right: 10px", true, true), "background: url(/foo/bar-rtl.png); left: 10px");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnDashLtrMultiplePropertiesWithDirection()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-left.png); direction: ltr", true, true), "background: url(/foo/bar-right.png); direction: rtl");
        }
        [TestMethod]
        public void FlipsUrlsWhenFlagsAreOnDashLtrRightMultiplePropertiesWithDirection()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar-ltr_left.png);right:10px; direction: ltr", true, true), "background: url(/foo/bar-rtl_right.png);left:10px; direction: rtl");
        }



        [TestMethod]
        public void FlipsPaddingLeft()
        {
            Assert.AreEqual(Flip("padding-left: bar"), "padding-right: bar");
        }
        [TestMethod]
        public void FlipsPaddingRight()
        {
            Assert.AreEqual(Flip("padding-right: bar"), "padding-left: bar");
        }


        [TestMethod]
        public void FlipsMarginLeft()
        {
            Assert.AreEqual(Flip("margin-left: bar"), "margin-right: bar");
        }
        [TestMethod]
        public void FlipsMarginRight()
        {
            Assert.AreEqual(Flip("margin-right: bar"), "margin-left: bar");
        }


        [TestMethod]
        public void FlipsBorderLeft()
        {
            Assert.AreEqual(Flip("border-left: bar"), "border-right: bar");
        }
        [TestMethod]
        public void FlipsBorderRight()
        {
            Assert.AreEqual(Flip("border-right: bar"), "border-left: bar");
        }


        [TestMethod]
        public void FlipsCursorWest()
        {
            Assert.AreEqual(Flip("cursor: w-resize"), "cursor: e-resize");
        }
        [TestMethod]
        public void FlipsCursorEast()
        {
            Assert.AreEqual(Flip("cursor: e-resize"), "cursor: w-resize");
        }
        [TestMethod]
        public void FlipsCursorSouthEast()
        {
            Assert.AreEqual(Flip("cursor: se-resize"), "cursor: sw-resize");
        }
        [TestMethod]
        public void FlipsCursorSouthWest()
        {
            Assert.AreEqual(Flip("cursor: sw-resize"), "cursor: se-resize");
        }
        [TestMethod]
        public void FlipsCursorNorthEast()
        {
            Assert.AreEqual(Flip("cursor: ne-resize"), "cursor: nw-resize");
        }
        [TestMethod]
        public void FlipsCursorNorthWest()
        {
            Assert.AreEqual(Flip("cursor: nw-resize"), "cursor: ne-resize");
        }


        [TestMethod]
        public void FlipsKeywordBackgroundImagePositionsRightTop()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar.png) right top"), "background: url(/foo/bar.png) left top");
        }
        [TestMethod]
        public void FlipsKeywordBackgroundImagePositionsLeftTop()
        {
            Assert.AreEqual(Flip("background: url(/foo/bar.png) left top"), "background: url(/foo/bar.png) right top");
        }
        [TestMethod]
        public void FlipsKeywordBackgroundPositionsRightTop()
        {
            Assert.AreEqual(Flip("background-position: right top"), "background-position: left top");
        }
        [TestMethod]
        public void FlipsKeywordBackgroundPositionsLeftTop()
        {
            Assert.AreEqual(Flip("background-position: left top"), "background-position: right top");
        }
        [TestMethod]
        public void FlipsKeywordBackgroundPositionsLeftNegative()
        {
            Assert.AreEqual(Flip("background-position: left -5"), "background-position: right -5");
        }
        [TestMethod]
        public void FlipsKeywordBackgroundPositionsLeftPositive()
        {
            Assert.AreEqual(Flip("background-position: left 5"), "background-position: right 5");
        }


        [TestMethod]
        public void FlipsPercentageBackgroundPositionsNothingToAll()
        {
            Assert.AreEqual(Flip("background-position: 0% 40%"), "background-position: 100% 40%");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsAllToNothing()
        {
            Assert.AreEqual(Flip("background-position: 100% 40%"), "background-position: 0% 40%");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMidRangeNumber()
        {
            Assert.AreEqual(Flip("background-position: 77% 0"), "background-position: 23% 0");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMidRangeAuto()
        {
            Assert.AreEqual(Flip("background-position: 77% auto"), "background-position: 23% auto");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMidRangeX()
        {
            Assert.AreEqual(Flip("background-position-x: 77%"), "background-position-x: 23%");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMidRangeY()
        {
            Assert.AreEqual(Flip("background-position-y: 23%"), "background-position-y: 23%");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsBackgroundShorthand()
        {
            Assert.AreEqual(Flip("background:url(../foo-bar_baz.2008.gif) no-repeat 25% 50%"), "background:url(../foo-bar_baz.2008.gif) no-repeat 75% 50%");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMultipleBackgroundShorthand()
        {
            Assert.AreEqual(Flip(".test { background: 90% 20% } .test2 { background: 60% 30% }"), ".test { background: 10% 20% } .test2 { background: 40% 30% }");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMultipleBackgroundShorthandAnother()
        {
            Assert.AreEqual(Flip(".foo { background: 90% 20% } .bar { background: 60% 30% }"), ".foo { background: 10% 20% } .bar { background: 40% 30% }");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMultipleBackgroundShorthandYetAnother()
        {
            Assert.AreEqual(Flip(".foo { background: 100% 20% } .bar { background: 60% 30% }"), ".foo { background: 0% 20% } .bar { background: 40% 30% }");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMultipleBackgroundShorthandDoesnNotModifyOtherStyle()
        {
            Assert.AreEqual(Flip(".foo { background: #777 } .bar{ margin: 0 5% 4% 0 }"), ".foo { background: #777 } .bar{ margin: 0 0 4% 5% }");
        }
        [TestMethod]
        public void FlipsPercentageBackgroundPositionsMultipleBackgroundShorthandDoesnNotModifyOtherStyleSingle()
        {
            Assert.AreEqual(Flip(".foo { background: #777; margin: 0 5% 4% 0 }"), ".foo { background: #777; margin: 0 0 4% 5% }");
        }

        [TestMethod]
        public void LeavesClassNamesAloneLeft()
        {
            Assert.AreEqual(Flip(".column-left { float: right }"), ".column-left { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneBright()
        {
            Assert.AreEqual(Flip("#bright-light { float: right }"), "#bright-light { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneDotLeft()
        {
            Assert.AreEqual(Flip("a.left:hover { float: right }"), "a.left:hover { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneBrightLeftNewline()
        {
            Assert.AreEqual(Flip("#bright-left,\n.test-me { float: right }"), "#bright-left,\n.test-me { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneBrightLeft()
        {
            Assert.AreEqual(Flip("#bright-left, .test-me { float: right }"), "#bright-left, .test-me { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneMultipleNamesAndCommas()
        {
            Assert.AreEqual(Flip("div.leftpill, div.leftpillon {margin-left: 0 !important}"), "div.leftpill, div.leftpillon {margin-right: 0 !important}");
        }
        [TestMethod]
        public void LeavesClassNamesAloneMultipleNamesAndCommasArrow()
        {
            Assert.AreEqual(Flip("div.left > span.right+span.left { float: right }"), "div.left > span.right+span.left { float: left }");
        }
        [TestMethod]
        public void LeavesClassNamesAloneMultipleNamesAndCommasLeftClass()
        {
            Assert.AreEqual(Flip(".thisclass .left .myclass {background:#fff;}"), ".thisclass .left .myclass {background:#fff;}");
        }
        [TestMethod]
        public void LeavesClassNamesAloneMultipleNamesAndCommasLeftClassAndMyId()
        {
            Assert.AreEqual(Flip(".thisclass .left .myclass #myid {background:#fff;}"), ".thisclass .left .myclass #myid {background:#fff;}");
        }


        [TestMethod]
        public void WorksOnMultipleRules()
        {
            Assert.AreEqual(Flip("body{direction:ltr;float:left}.b2{direction:ltr;float:left}"), "body{direction:rtl;float:right}.b2{direction:rtl;float:right}");
        }


        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsDiv()
        {
            Assert.AreEqual(Flip("/* @noflip */ div { float: left; }"), "/* @noflip */ div { float: left; }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsNoThenYes()
        {
            Assert.AreEqual(Flip("/* @noflip */ div { float: left; } div { float: right; }"), "/* @noflip */ div { float: left; } div { float: left; }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsNoThenYesNewline()
        {
            Assert.AreEqual(Flip("/* @noflip */\ndiv { float: left; }\ndiv { float: right; }"), "/* @noflip */\ndiv { float: left; }\ndiv { float: left; }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsWithinClassSecondOfTwo()
        {
            Assert.AreEqual(Flip("div { float: right; /* @noflip */ float: left; }"), "div { float: left; /* @noflip */ float: left; }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsWithinClassSecondOfThree()
        {
            Assert.AreEqual(Flip("div\n{ float: right;\n/* @noflip */\n float: left;\n }"), "div\n{ float: left;\n/* @noflip */\n float: left;\n }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsWithinClassSecondOfTwoOwnLine()
        {
            Assert.AreEqual(Flip("div\n{ float: right;\n/* @noflip */\n text-align: left\n }"), "div\n{ float: left;\n/* @noflip */\n text-align: left\n }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsWithinClassFirstOfTwo()
        {
            Assert.AreEqual(Flip("div\n{ float: right;\n/* @noflip */\n text-align: left\n }"), "div\n{ float: left;\n/* @noflip */\n text-align: left\n }");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipCommentsWithinClassFirstOfTwoOwnLine()
        {
            Assert.AreEqual(Flip("div\n{ /* @noflip */\ntext-align: left;\nfloat: right\n	}"), "div\n{ /* @noflip */\ntext-align: left;\nfloat: left\n	}");
        }
        [TestMethod]
        public void DoesNotFlipRulesOrPropertiesWithNoFlipAllElementsTwoDivs()
        {
            Assert.AreEqual(Flip("/* @noflip */div{float:left;text-align:left;}div{float:right}"), "/* @noflip */div{float:left;text-align:left;}div{float:left}");
        }



        [TestMethod]
        public void FlipsBorderRadiusNotation()
        {
            Assert.AreEqual(Flip("border-radius: 15px .25em 0ex 0pt"), "border-radius: .25em 15px 0pt 0ex");
        }
        [TestMethod]
        public void FlipsBorderRadiusNotationFour()
        {
            Assert.AreEqual(Flip("border-radius: 15px 10px 15px 0px"), "border-radius: 10px 15px 0px 15px");
        }
        [TestMethod]
        public void FlipsBorderRadiusNotationTwo()
        {
            Assert.AreEqual(Flip("border-radius: 8px 7px"), "border-radius: 7px 8px");
        }
        [TestMethod]
        public void FlipsBorderRadiusNotationOne()
        {
            Assert.AreEqual(Flip("border-radius: 5px"), "border-radius: 5px");
        }



        [TestMethod]
        public void FlipsMozGradientNotation()
        {
            Assert.AreEqual(Flip("background-image: -moz-linear-gradient(#326cc1, #234e8c)"), "background-image: -moz-linear-gradient(#326cc1, #234e8c)");
        }
        [TestMethod]
        public void FlipsWebkitGradientNotation()
        {
            Assert.AreEqual(Flip("background-image: -webkit-gradient(linear, 100% 0%, 0% 0%, from(#666666), to(#ffffff))"), "background-image: -webkit-gradient(linear, 100% 0%, 0% 0%, from(#666666), to(#ffffff))");
        }
        [TestMethod]
        public void FlipsGradientNotation()
        {
            Assert.AreEqual(Flip("background-image: linear-gradient(#326cc1, #234e8c)"), "background-image: linear-gradient(#326cc1, #234e8c)");
        }




        private string Flip(string code, bool swapLtrRtlInUrl = false, bool swapLeftRightInUrl = false)
        {
            return cssJanus.Transform(code, swapLtrRtlInUrl, swapLeftRightInUrl);
        }
    }
}
