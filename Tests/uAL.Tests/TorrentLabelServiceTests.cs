using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uAL.Services;

namespace uAL.Tests
{
    [TestClass]
    public class TorrentLabelServiceTests
    {
        [TestMethod]
        public void CreateTorrentLabel_SimpleChild_GetSimpleLabel()
        {
            var label = TorrentLabelService.CreateTorrentLabel(@"C:\Torrents\",@"C:\Torrents\TV\Arrow\Arrow.torrent");
            Assert.AreEqual(@"TV\Arrow", label);
        }

        [TestMethod]
        public void CreateTorrentLabel_TwoLevel_GetLabel()
        {
            var label = TorrentLabelService.CreateTorrentLabel(@"C:\Torrents\", @"C:\Torrents\Movies\SomeMovie.torrent");
            Assert.AreEqual(@"Movies", label);
        }
    }
}
