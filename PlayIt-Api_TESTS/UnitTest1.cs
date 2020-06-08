using System;
using Xunit;
using PlayIt_Api;
using PlayIt_Api.Services.Security.Account;
using PlayIt_Api.Services.Security;

namespace PlayIt_Api_TESTS
{
    public class UnitTest1
    {
        [Fact]
        public void password_follows_password_pattern_when_created()
        {
            IPasswordService ps = new PasswordService(new SHA512HashingService(), 8, 32);

            byte[] s;

            //correct character combination: number, special, upper and lower case letter
            Assert.True(ps.CreatePassword("Test123@bc", out s) != null);

            //password empty
            Assert.Throws<ArgumentNullException>(() => ps.CreatePassword("", out s));
            //password null
            Assert.Throws<ArgumentNullException>(() => ps.CreatePassword(null, out s));
            //too short
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("ki", out s));
            //too long
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("2314353453452345234523452345345345345345tg5446un 47j7%¤/#B&/B#B&7n67567657474756756754745", out s));
            //invalid character combinations
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("abcdefg1234", out s));
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("!QWerdggh", out s));
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("avbcvb!!", out s));
            Assert.Throws<ArgumentOutOfRangeException>(() => ps.CreatePassword("invt&&!!1", out s));
        }

        [Fact]
        public void password_service_generates_random_salt()
        {
            IPasswordService ps = new PasswordService(new SHA512HashingService(), 8, 32);
            byte[][] salts = new byte[100][];

            //generate salts
            for (int i = 0; i < 100; i++)
            {
                ps.CreatePassword("Test123@", out salts[i]);
            }

            //compare all salt values
            bool saltsMatch = false;
            for (int i = 0; i < 100; i++)
            {
                for (int s = 0; s < 100; s++)
                {
                    //salt on the same index will be the same ofcourse, so skip this
                    if (i == s)
                        continue;

                    for (int a = 0; a < 32; a++)
                    {
                        //find different salt byte
                        if (salts[i][a] != salts[s][a])
                            break;

                        //if salt is the same, set to true
                        if (a == 31)
                            saltsMatch = true;
                    }
                }
            }

            Assert.False(saltsMatch);
        }

        [Fact]
        public void password_service_compares_passwords_correctly()
        {
            IPasswordService ps = new PasswordService(new SHA512HashingService(), 8, 32);
            string rawPassword = "Test123@!bc";

            byte[]
                s1,
                s2,
                s3;

            byte[]
                h1 = ps.CreatePassword(rawPassword, out s1),
                h2 = ps.CreatePassword(rawPassword, out s2),
                h3 = ps.CreatePassword(rawPassword, out s3);

            //passwords should match with their correct salt
            Assert.True(ps.ComparePasswords(rawPassword, h1, s1));
            Assert.True(ps.ComparePasswords(rawPassword, h2, s2));
            Assert.True(ps.ComparePasswords(rawPassword, h3, s3));
            //passwords shouldn't match with incorrect salt
            Assert.False(ps.ComparePasswords(rawPassword, h1, s2));
            Assert.False(ps.ComparePasswords(rawPassword, h1, s3));
            Assert.False(ps.ComparePasswords(rawPassword, h2, s1));
            Assert.False(ps.ComparePasswords(rawPassword, h2, s3));
            Assert.False(ps.ComparePasswords(rawPassword, h3, s1));
            Assert.False(ps.ComparePasswords(rawPassword, h3, s2));
        }

        [Fact]
        public void user_service_creates_new_user_correctly()
        {

        }

        [Fact]
        public void user_gets_verified_correctly()
        {

        }
    }
}
