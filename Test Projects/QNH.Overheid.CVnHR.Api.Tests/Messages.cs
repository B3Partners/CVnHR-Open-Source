using System;
using System.Collections.Generic;
using System.Text;

namespace QNH.Overheid.CVnHR.Api.Tests
{
    public static class Messages
    {
        public const string OldNieuweInschrijving = @"<ns2:NieuweInschrijving xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2016/01"" xmlns=""http://schemas.kvk.nl/schemas/hrip/signaal/2016/01"" xmlns:ns3=""http://schemas.kvk.nl/schemas/hrip/signaal/insolventiewijziging/2016/01"" xmlns:ns4=""http://schemas.kvk.nl/schemas/hrip/signaal/rechtsvormwijziging/2016/01"" xmlns:ns5=""http://schemas.kvk.nl/schemas/hrip/signaal/voortzettingenoverdracht/2016/01"">
      <signaalId>5e345d3b-3dad-415e-815a-60cf362765f7</signaalId>
      <registratieId>-7a931e29:154f621a107:-112f</registratieId>
      <registratieTijdstip>20160528085600994</registratieTijdstip>
      <ns2:startdatum>20160528</ns2:startdatum>
      <ns2:heeftBetrekkingOp>
        <kvkNummer>28065255</kvkNummer>
        <nonMailing>
          <code>J</code>
          <omschrijving>Ja</omschrijving>
        </nonMailing>
        <totaalWerkzamePersonen>28</totaalWerkzamePersonen>
        <heeftAlsEigenaar>
          <rechtsPersoon>
            <persoonRechtsvorm>Besloten Vennootschap</persoonRechtsvorm>
            <rsin>803111496</rsin>
            <bezoekAdres>
              <postcode>
                <cijfercombinatie>1105</cijfercombinatie>
                <lettercombinatie>BH</lettercombinatie>
              </postcode>
            </bezoekAdres>
          </rechtsPersoon>
        </heeftAlsEigenaar>
        <wordtUitgeoefendIn>
          <vestigingsnummer>000019979282</vestigingsnummer>
          <bezoekAdres>
            <postcode>
              <cijfercombinatie>1105</cijfercombinatie>
              <lettercombinatie>BH</lettercombinatie>
            </postcode>
          </bezoekAdres>
          <activiteit>
            <sbiCode>6420</sbiCode>
            <isHoofdactiviteit>
              <code>J</code>
              <omschrijving>Ja</omschrijving>
            </isHoofdactiviteit>
          </activiteit>
          <isHoofdvestiging>
            <code>J</code>
            <omschrijving>Ja</omschrijving>
          </isHoofdvestiging>
        </wordtUitgeoefendIn>
      </ns2:heeftBetrekkingOp>
      <ns2:aanleiding>
        <ns2:omschrijving>Nieuw</ns2:omschrijving>
      </ns2:aanleiding>
    </ns2:NieuweInschrijving>";

        public const string NewNieuweInschrijving = @"<NieuweInschrijving xmlns=""http://schemas.kvk.nl/schemas/hrip/nieuweinschrijving/2016/01"">
      <berichtId xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"">cb52d6e8-4f2f-4c09-a6ea-7edc5acd6fe4</berichtId>
      <registratieId xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"">-761bb9eb:1552ecd1bc8:-659f</registratieId>
      <registratieTijdstip xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"">20160608090151121</registratieTijdstip>
      <heeftBetrekkingOp xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"">
        <kvkNummer>66139546</kvkNummer>
        <nonMailing>Nee</nonMailing>
        <heeftAlsEigenaar>
          <rechtspersoon>
            <persoonRechtsvorm>Vereniging</persoonRechtsvorm>
            <rsin>856599402</rsin>
            <bezoekadres>
              <postcode>
                <cijfercombinatie>2342</cijfercombinatie>
                <lettercombinatie>YY</lettercombinatie>
              </postcode>
              <afgeschermd>Nee</afgeschermd>
            </bezoekadres>
          </rechtspersoon>
        </heeftAlsEigenaar>
        <wordtUitgeoefendIn>
          <vestigingsnummer>000035320265</vestigingsnummer>
          <bezoekadres>
            <postcode>
              <cijfercombinatie>2342</cijfercombinatie>
              <lettercombinatie>YY</lettercombinatie>
            </postcode>
            <afgeschermd>Nee</afgeschermd>
          </bezoekadres>
          <activiteit>
            <sbiCode>0129</sbiCode>
            <isHoofdactiviteit>Ja</isHoofdactiviteit>
          </activiteit>
          <isHoofdvestiging>Ja</isHoofdvestiging>
        </wordtUitgeoefendIn>
      </heeftBetrekkingOp>
      <ns2:startdatum xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"">20151211</ns2:startdatum>
      <ns2:aanleiding xmlns:ns2=""http://schemas.kvk.nl/schemas/hrip/signaal/nieuweinschrijving/2017/01"" xmlns:dgl=""http://www.digilevering.nl/digilevering.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.kvk.nl/schemas/hrip/bericht/2017/01"">
        <code>NW</code>
        <omschrijving>Nieuw</omschrijving>
      </ns2:aanleiding>
    </NieuweInschrijving>";

    }
}
