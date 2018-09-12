﻿//
// Copyright (c) 2018
// Licensed under the Academic Free License version 3.0
//
// History:
//   16 August 2018 Ian Davies Creation based on Java Toolkit at same time from project-haystack.org downloads
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectHaystack;
using ProjectHaystack.io;

namespace ProjectHaystackTest
{
    [TestClass]
    public abstract class HValTest : HaystackTest
    {
        [TestMethod]
        protected void verifyZinc(HVal val, string s)
        {
            Assert.AreEqual(val.toZinc(), s);
            Assert.IsTrue(read(s).hequals(val));
        }

        protected HVal read(string s)
        {
            return new HZincReader(s).readVal();
        }
    }
}