using System.Reflection;
using FluentAssertions;
using Migrator.Framework;
using Moq;
using NLog;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    [Ignore]
    public class MigrationLoaderTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var providerMock = new Mock<TransformationProviderBase>();

            providerMock.Setup(c => c.Logger).Returns(LogManager.GetCurrentClassLogger);

            _migrationLoader = new MigrationLoader(providerMock.Object, Assembly.GetExecutingAssembly());
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.FirstMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.SecondMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.ThirdMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.ForthMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.BadMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.SixthMigration));
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.NonIgnoredMigration));
        }

        #endregion

        private MigrationLoader _migrationLoader;


        [Test]
        public void CheckForDuplicatedVersion()
        {
            _migrationLoader.MigrationsTypes.Add(typeof (MigratorTest.FirstMigration));
            Assert.Throws<DuplicatedVersionException>(() => _migrationLoader.CheckForDuplicatedVersion());
        }

        [Test]
        public void LastVersion()
        {
            _migrationLoader.LastVersion.Should().Be(7);
        }

        [Test]
        public void NullIfNoMigrationForVersion()
        {
            _migrationLoader.GetMigration(99999999).Should().BeNull();
        }

        [Test]
        public void ZeroIfNoMigrations()
        {
            _migrationLoader.MigrationsTypes.Clear();
            _migrationLoader.LastVersion.Should().Be(0);
        }
    }
}