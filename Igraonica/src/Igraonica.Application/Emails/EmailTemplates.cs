using Igraonica.Domain.Entities;

namespace Igraonica.Application.Emails;

/// <summary>
/// Gradi HTML sadrzaj poruka elektronske poste. Poruke se ne salju
/// neposredno vec se upisuju u tabelu OutboxMessages (videti IOutboxRepository),
/// cime je ispunjen nefunkcionalni zahtev NFZ-07.
/// </summary>
public static class EmailTemplates
{
    private const string Wrapper = """
        <div style="font-family:Arial,Helvetica,sans-serif;max-width:520px;margin:0 auto;
                    background:#ffffff;border-radius:16px;overflow:hidden;border:1px solid #e1e3f0;">
          <div style="background:#0c1024;padding:28px 24px;text-align:center;">
            <h1 style="color:#3fbf22;margin:0;font-size:22px;letter-spacing:0.5px;">JOYLAND</h1>
            <p style="color:#ffffffb3;margin:4px 0 0;font-size:13px;">Igraonica &middot; Karadjordjeva 18, Smederevo</p>
          </div>
          <div style="padding:28px;color:#131629;font-size:15px;line-height:1.6;">
            {0}
          </div>
          <div style="padding:16px 28px;background:#f4f5fb;color:#676c93;font-size:12px;text-align:center;">
            Ovo je automatska poruka sistema za rezervaciju termina. Molimo ne odgovarajte na nju.<br/>
            063 645 886 &middot; igraonicajoyland@gmail.com
          </div>
        </div>
        """;

    private static string Wrap(string bodyHtml) => string.Format(Wrapper, bodyHtml);

    private static string Button(string href, string label) => $"""
        <p style="text-align:center;margin:24px 0;">
          <a href="{href}" style="background:#7d50c5;color:#fff;padding:12px 24px;
             border-radius:999px;text-decoration:none;font-weight:bold;display:inline-block;">{label}</a>
        </p>
        """;

    private static string DetailRow(string label, string value) => $"""
        <tr>
          <td style="padding:6px 0;color:#676c93;">{label}</td>
          <td style="padding:6px 0;font-weight:bold;text-align:right;">{value}</td>
        </tr>
        """;

    private static string DetailsTable(string rows) => $"""
        <table style="width:100%;border-collapse:collapse;background:#f4f5fb;border-radius:12px;
                       padding:16px;margin:16px 0;">
          <tbody>{rows}</tbody>
        </table>
        """;

    public static (string Subject, string Body) EmailVerification(string firstName, string verifyUrl) => (
        "Potvrdite svoju adresu elektronske poste",
        Wrap($"""
            <p>Zdravo {firstName},</p>
            <p>Hvala Vam na registraciji u sistem za rezervaciju rodjendanskih proslava
            igraonice JoyLand. Da biste mogli da napravite rezervaciju, potrebno je da
            potvrdite svoju adresu elektronske poste.</p>
            {Button(verifyUrl, "Potvrdi adresu")}
            <p style="color:#676c93;font-size:13px;">Ukoliko dugme ne radi, kopirajte sledecu adresu u
            pregledac:<br/>{verifyUrl}</p>
            """));

    public static (string Subject, string Body) PasswordReset(string firstName, string resetUrl) => (
        "Zahtev za promenu lozinke",
        Wrap($"""
            <p>Zdravo {firstName},</p>
            <p>Primili smo zahtev za promenu lozinke na Vasem nalogu. Kliknite na dugme
            ispod da biste postavili novu lozinku. Ako niste Vi poslali ovaj zahtev,
            slobodno zanemarite ovu poruku.</p>
            {Button(resetUrl, "Postavi novu lozinku")}
            """));

    public static (string Subject, string Body) ReservationCreated(Reservation reservation) => (
        $"Zakazali ste termin u igraonici JoyLand – {reservation.ReservationNumber}",
        Wrap($"""
            <p>Zdravo!</p>
            <p>Zakazali ste termin za proslavu deteta <strong>{reservation.ChildName}</strong>
            dana <strong>{reservation.EventDate:dd.MM.yyyy.}</strong> u igraonici JoyLand.</p>
            {DetailsTable(
                DetailRow("Broj rezervacije", reservation.ReservationNumber) +
                DetailRow("Datum proslave", $"{reservation.EventDate:dd.MM.yyyy.}") +
                DetailRow("Broj dece", reservation.NumberOfChildren.ToString()) +
                DetailRow("Ukupna cena", $"{reservation.TotalPrice:N0} RSD"))}
            <p><strong>Vazno:</strong> rezervacija ce biti automatski otkazana ukoliko do
            <strong>{reservation.ConfirmationDeadline:dd.MM.yyyy. HH:mm}</strong> licno ne
            dodjete u igraonicu radi potvrde termina.</p>
            <p>Vidimo se uskoro!<br/>Ekipa JoyLand</p>
            """));

    public static (string Subject, string Body) ReservationConfirmed(Reservation reservation) => (
        $"Rezervacija {reservation.ReservationNumber} je potvrdjena",
        Wrap($"""
            <p>Zdravo!</p>
            <p>Vas dolazak je zabelezen i rezervacija broj
            <strong>{reservation.ReservationNumber}</strong> za proslavu deteta
            <strong>{reservation.ChildName}</strong> je potvrdjena.</p>
            {DetailsTable(
                DetailRow("Datum proslave", $"{reservation.EventDate:dd.MM.yyyy.}") +
                DetailRow("Ukupna cena", $"{reservation.TotalPrice:N0} RSD"))}
            <p>Vidimo se na proslavi!<br/>Ekipa JoyLand</p>
            """));

    public static (string Subject, string Body) ReservationCancelled(Reservation reservation, string reason) => (
        $"Rezervacija {reservation.ReservationNumber} je otkazana",
        Wrap($"""
            <p>Zdravo,</p>
            <p>Rezervacija broj <strong>{reservation.ReservationNumber}</strong> za
            proslavu deteta <strong>{reservation.ChildName}</strong> dana
            <strong>{reservation.EventDate:dd.MM.yyyy.}</strong> je otkazana.</p>
            <p style="color:#676c93;">Razlog: {reason}</p>
            """));

    public static (string Subject, string Body) ReservationAutoExpired(Reservation reservation) => (
        $"Rezervacija {reservation.ReservationNumber} je istekla",
        Wrap($"""
            <p>Zdravo,</p>
            <p>Rezervacija broj <strong>{reservation.ReservationNumber}</strong> je
            automatski ponistena jer nije fizicki potvrdjena u roku od pet dana.
            Termin je ponovo slobodan za druge zainteresovane.</p>
            """));
}
