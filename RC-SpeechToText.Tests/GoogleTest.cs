using Microsoft.VisualStudio.TestTools.UnitTesting;
using RC_SpeechToText.Controllers;

namespace RC_SpeechToText.Tests
{
    [TestClass]
    public class GoogleTest
    {
        //[TestMethod]
        //public void TestCreateManualTranscript()
        //{
        //    string actual = Helpers.CreateManualTranscipt("..\\..\\..\\..\\RC-SpeechToText\\Audio\\RAD_VEGAN\\RAD_Vegan_456-491.srt");

        //    string expected = "Si au bout du processus la démonstration est faite de manière objective, qu’il n’y a pas de violation à la loi." +
        //        " Parfait. Mais si ce n’est pas le cas et qu’on ne fait rien, ben les tribunaux seront là toujours pour faire assurer le respect de la loi." +
        //        " - Demain matin, un monde végane, est-ce que c’est possible? - Non. - Pourquoi? - Y’a des transitions qui sont nécessaires." +
        //        " On ne peut pas du jour au lendemain fermer les abattoirs. Certains végétaliens ont choisi ce mode de vie pour des raisons qui n’ont rien à voir" +
        //        " avec le bien-être animal mais leur propre bien-être. Ou pour des raisons environnementales. Alors les causes convergent. ";

        //    Assert.AreEqual(expected, actual);
        //}

        [TestMethod]
        public void TestCalculateAccuracy()
        {
            string text1 = "hello world this is a test of accuracy.";
            string text2 = "hello world this is a test of accuracy.";

            double accuracy = Helpers.CalculateAccuracy(text1, text2);

            Assert.AreEqual(1.0, accuracy);
        }
    }
}
