﻿using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectHaystack;
using ProjectHaystack.io;

namespace ProjectHaystackTest.io
{
    [TestClass]
    public class TrioWriterTests
    {
        [TestMethod]
        public void WriteEntity_SafeString_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["strTag"] = new HaystackString("OK if unquoted if only safe chars"),
                });

                // Act.
                trioWriter.WriteEntity(entity);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
strTag:OK if unquoted if only safe chars
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntity_UnsafeString_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["strTag"] = new HaystackString("Not ok if unquoted (with unsafe chars)."),
                    ["guid"] = new HaystackString("cc49e18d-bef0-446f-a106-17eeb7c2eb53"),
                });

                // Act.
                trioWriter.WriteEntity(entity);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
strTag:""Not ok if unquoted (with unsafe chars).""
guid:""cc49e18d-bef0-446f-a106-17eeb7c2eb53""
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntity_QuotedString_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["strTag"] = new HaystackString("Line with \"inline\" quotes."),
                });

                // Act.
                trioWriter.WriteEntity(entity);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
strTag:""Line with \""inline\"" quotes.""
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntity_MultiLineString_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["summary"] = new HaystackString("This is a string value which spans multiple\nlines with two or more space characters"),
                });

                // Act.
                trioWriter.WriteEntity(entity);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
summary:
  This is a string value which spans multiple
  lines with two or more space characters
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntity_MultiLineQuotedString_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["summary"] = new HaystackString("This is a string value with \"quotes\" or (unsafe) characters which spans multiple\nlines with two or more space characters"),
                });

                // Act.
                trioWriter.WriteEntity(entity);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
summary:
  This is a string value with ""quotes"" or (unsafe) characters which spans multiple
  lines with two or more space characters
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntities_TwoEntities_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity1 = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["dis"] = new HaystackString("Site 1"),
                    ["site"] = new HaystackMarker(),
                    ["area"] = new HaystackNumber(3702, "ft²"),
                    ["geoAddr"] = new HaystackString("100 Main St, Richmond, VA"),
                    ["geoCoord"] = new HaystackCoordinate(37.5458m, -77.4491m),
                    ["strTag"] = new HaystackString("OK if unquoted if only safe chars"),
                    ["summary"] = new HaystackString("This is a string value which spans multiple\nlines with two or more space characters"),
                });
                var entity2 = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["name"] = new HaystackString("Site 2"),
                    ["site"] = new HaystackMarker(),
                    ["summary"] = new HaystackString("Entities are separated by one or more dashes"),
                });

                // Act.
                trioWriter.WriteEntity(entity1);
                trioWriter.WriteEntity(entity2);
                var trio = writer.ToString();

                // Assert.
                var target = @"dis:Site 1
site
area:3702ft²
geoAddr:""100 Main St, Richmond, VA""
geoCoord:C(37.5458,-77.4491)
strTag:OK if unquoted if only safe chars
summary:
  This is a string value which spans multiple
  lines with two or more space characters
---
name:Site 2
site
summary:Entities are separated by one or more dashes
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }

        [TestMethod]
        public void WriteEntities_NestedGrid_IsValid()
        {
            using (var writer = new StringWriter())
            {
                // Arrange.
                var trioWriter = new TrioWriter(writer);
                var entity1 = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["type"] = new HaystackString("list"),
                    ["val"] = new HaystackList(new HaystackNumber(1), new HaystackNumber(2), new HaystackNumber(3)),
                });
                var entity2 = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["type"] = new HaystackString("dict"),
                    ["val"] = new HaystackDictionary(new Dictionary<string, HaystackValue>
                    {
                        ["dis"] = new HaystackString("Dict!"),
                        ["foo"] = new HaystackMarker(),
                    }),
                });
                var grid = new HaystackGrid()
                    .AddColumn("b")
                    .AddColumn("a")
                    .AddRow(new HaystackNumber(20), new HaystackNumber(10));
                var entity3 = new HaystackDictionary(new Dictionary<string, HaystackValue>
                {
                    ["type"] = new HaystackString("grid"),
                    ["val"] = grid,
                });

                // Act.
                trioWriter.WriteComment("Trio");
                trioWriter.WriteEntity(entity1);
                trioWriter.WriteEntity(entity2);
                trioWriter.WriteEntity(entity3);
                var trio = writer.ToString();

                // Assert.
                var target = @"// Trio
type:list
val:[1,2,3]
---
type:dict
val:{dis:""Dict!"" foo}
---
type:grid
val:Zinc:
  ver:""3.0""
  b,a
  20,10
";
                Assert.AreEqual(target.Replace("\r", ""), trio.Replace("\r", ""));
            }
        }
    }
}