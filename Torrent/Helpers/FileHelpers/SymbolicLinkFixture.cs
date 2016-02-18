using System.IO;
using NUnit.Framework;
 
namespace Torrent.Helpers.FileHelpers
{
    [TestFixture]
    public class SymbolicLinkFixture
    {
        string testFilePath;
 
        string testFolderPath;
 
        string workingDirectory;
 
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            AssertMore.IgnoreIf2003();
        }
 
        [SetUp]
        public void SetUp()
        {
            workingDirectory = IOHelper.CreateMyTempPath(this);
 
            testFolderPath = Path.Combine(workingDirectory, "foo");
            Directory.CreateDirectory(testFolderPath);
 
            testFilePath = Path.Combine(testFolderPath, "bar.txt");
            IOHelper.CreateTestFile(testFilePath);
        }
 
        [TearDown]
        public void TearDown()
        {
            IOHelper.DeleteMyTempPath(this);
        }
 
        private void assertDirLink(string linkPath, string targetPath)
        {
            SymbolicLink.CreateDirectoryLink(linkPath, targetPath);
            Assert.IsTrue(Directory.Exists(linkPath),
                "Link was not created?  Dir does not exist: " + linkPath);
            string expectedLinkedFilePath = Path.Combine(linkPath, "bar.txt");
            AssertMore.FilesAreEqual(testFilePath, expectedLinkedFilePath);
 
            Assert.IsTrue(SymbolicLink.Exists(linkPath),
                "the link was not detected as a symlink");
            Assert.IsFalse(JunctionPoint.Exists(linkPath),
                "the link was detected as a junction point");
 
            string actual = SymbolicLink.GetTarget(linkPath);
            Assert.AreEqual(targetPath, actual,
                "The retrieved target does not match what I created");
        }
 
        private void assertFileLink(string linkPath, string target)
        {
            SymbolicLink.CreateFileLink(linkPath, target);
            AssertMore.FileContentsAreEqual(testFilePath, linkPath);
 
            Assert.IsTrue(SymbolicLink.Exists(linkPath),
                "the link was not detected as a symlink");
        }
 
        [Test]
        public void CanCreateSymLinkToAbsoluteDirectoryPath()
        {
            string linkPath = Path.Combine(workingDirectory, "link-to-foo");
            assertDirLink(linkPath, testFolderPath);
        }
 
        [Test]
        public void CanCreateSymLinkToRelativeDirectoryPath()
        {
            string linkPath = Path.Combine(workingDirectory, "link-to-foo");
            assertDirLink(linkPath, "foo");
        }
 
        [Test]
        public void CanCreateSymlinkToParentDirectoryPath()
        {
            DirectoryInfo subDirectory = Directory.CreateDirectory(
                Path.Combine(workingDirectory, @"buried\under\folders"));
            string linkPath = Path.Combine(subDirectory.FullName, "link-to-foo");
            assertDirLink(linkPath, @"..\..\..\foo");
        }
 
        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void CanCreateSymLinkToMissingTargetPath()
        {
            string linkPath = Path.Combine(workingDirectory, "link-to-nowhere");
            SymbolicLink.CreateDirectoryLink(linkPath, @"..\nothing\here");
            Assert.IsTrue(Directory.Exists(linkPath),
                "Symlink not created?  Path: " + linkPath);
            Directory.GetFiles(linkPath);
        }
 
        [Test]
        public void CanCreateLinkToAbsoluteFilePath()
        {
            string linkPath = Path.Combine(testFolderPath, "foolink.txt");
            assertFileLink(linkPath, testFilePath);
        }
 
        [Test]
        public void CanCreateLinkToRelativeFilePath()
        {
            string linkPath = Path.Combine(testFolderPath, "fooLink.txt");
            assertFileLink(linkPath, "bar.txt");
        }
 
        [Test]
        public void CanCreateLinkToParentRelativeFilePath()
        {
            DirectoryInfo newDir = Directory.CreateDirectory(
                Path.Combine(workingDirectory, @"buried\under\folders"));
            string linkPath = Path.Combine(newDir.FullName, "fooLink.txt");
            assertFileLink(linkPath, @"..\..\..\foo\bar.txt");
        }
 
        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void CanCreateLinkToMissingFilePath()
        {
            string linkPath = Path.Combine(workingDirectory, "link-to-nowhere.txt");
            SymbolicLink.CreateFileLink(linkPath, @"..\nothing\here.txt");
            Assert.IsTrue(File.Exists(linkPath),
                "Did not create the symlink?  Path: " + linkPath);
            IOHelper.GetFileData(linkPath);
        }
 
        [Test]
        public void SymLinkTestsFalseForRealDirectory()
        {
            Assert.IsFalse(SymbolicLink.Exists(testFolderPath));
        }
 
        [Test]
        public void SymLinkTestsFalseForRealFile()
        {
            Assert.IsFalse(SymbolicLink.Exists(testFilePath));
        }
 
        [Test]
        public void SymLinkTestFalseForJunctionPoint()
        {
            string linkPath = Path.Combine(workingDirectory, "link-to-foo");
            JunctionPoint.Create(linkPath, testFolderPath, true);
            Assert.IsTrue(Directory.Exists(linkPath), "link not created?");
            Assert.IsTrue(JunctionPoint.Exists(linkPath), "junction does not exists?");
            Assert.IsFalse(SymbolicLink.Exists(linkPath), "incorrectly tested as a symbolic link");
        }
 
        [Test]
        public void SymLinkTestsFalseIfNoSuchPath()
        {
            Assert.IsFalse(SymbolicLink.Exists(@"c:\foo\bar\baz"));
        }
 
        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void ExceptionIfAttemptToGetTargetForInvalidPath()
        {
            SymbolicLink.GetTarget(@"c:\foo\bar\baz");
        }
 
        [Test]
        [ExpectedException(typeof(IOException))]
        public void ExceptionOnAttemptingToReplaceAnExistingDirectory()
        {
            string linkPath = Path.Combine(testFolderPath, "bar");
            Directory.CreateDirectory(linkPath);
            SymbolicLink.CreateDirectoryLink(linkPath, "bleh");
        }
    }
}