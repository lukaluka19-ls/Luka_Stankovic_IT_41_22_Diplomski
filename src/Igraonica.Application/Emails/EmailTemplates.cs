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
                    background:#fefaf3;border-radius:16px;overflow:hidden;border:1px solid #f1d9b5;">
          <div style="background:#ff8fab;padding:24px;text-align:center;">
            <h1 style="color:#ffffff;margin:0;font-size:22px;">JoyLand Igraonica</h1>
            <p style="color:#fff0f5;margin:4px 0 0;font-size:13px;">Karadjordjeva 18, Smederevo</p>
          </div>
          <div style="padding:28px;color:#4a3f35;font-size:15px;line-height:1.6;">
            {0}
          </div>
          <div style="padding:16px 28px;background:#fff3e0;color:#9c8a76;font-size:12px;text-align:center;">
            Ovo je automatska poruka sistema za rezervaciju termina. Molimo ne odgovarajte na nju.
          </div>
        </div>
        """;

    private static string Wrap(string bodyHtml) => string.Format(Wrapper, bodyHtml);

    public static (string Subject, string Body) EmailVerification(string firstName, string verifyUrl) => (
        "Potvrdite svoju adresu elektronske poste",
        Wrap($"""
            <p>Zdravo {firstName},</p>
            <p>Hvala Vam na registraciji u sistem za rezervaciju rodjendanskih proslava
            igraonice JoyLand. Da biste mogli da napravite rezervaciju, potrebno je da
            potvrdite svoju adresu elektronske poste.</p>
            <p style="text-align:center;margin:24px 0;">
              <a href="{verifyUrl}" style="background:#ff8fab;color:#fff;padding:12px 24px;
                 border-radius:999px;text-decoration:none;font-weight:bold;">Potvrdi adresu</a>
            </p>
            <p>Ukoliko dugme ne radi, kopirajte sledecu adresu u pregledac:<br/>{verifyUrl}</p>
            """));

    public static (string Subject, string Body) PasswordReset(string firstName, string resetUrl) => (
        "Zahtev za promenu lozinke",
        Wrap($"""
            <p>Zdravo {firstName},</p>
            <p>Primili smo zahtev za promenu lozinke na Vasem nalogu. Kliknite na dugme
            ispod da biste postavili novu lozinku. Ako niste Vi poslali ovaj zahtev,
            slobodno zanemarite ovu poruku.</p>
            <p style="text-align:center;margin:24px 0;">
              <a href="{resetUrl}" style="background:#ff8fab;color:#fff;padding:12px 24px;
                 border-radius:999px;text-decoration:none;font-weight:bold;">Postavi novu lozinku</a>
            </p>
            """));

    public static (string Subject, string Body) ReservationCreated(Reservation reservation) => (
        $"Rezervacija {reservation.ReservationNumber} je kreirana",
        Wrap($"""
            <p>Zdravo,</p>
            <p>Vasa rezervacija broj <strong>{reservation.ReservationNumber}</strong> za
            proslavu deteta <strong>{reservation.ChildName}</strong> dana
            <strong>{reservation.EventDate:dd.MM.yyyy.}</strong> je uspesno kreirana.</p>
            <p><strong>Vazno:</strong> rezervacija ce biti otkazana ukoliko do
            <strong>{reservation.ConfirmationDeadline:dd.MM.yyyy. HH:mm}</strong> licno ne
            dodjete u igraonicu radi potvrde termina.</p>
            <p>Ukupna cena: <strong>{reservation.TotalPrice:N0} RSD</strong></p>
            """));

    public static (string Subject, string Body) ReservationConfirmed(Reservation reservation) => (
        $"Rezervacija {reservation.ReservationNumber} je potvrdjena",
        Wrap($"""
            <p>Zdravo,</p>
            <p>Vas dolazak je zabelezen i rezervacija broj
            <strong>{reservation.ReservationNumber}</strong> je potvrdjena. Vidimo se
            na proslavi <strong>{reservation.EventDate:dd.MM.yyyy.}</strong>!</p>
            """));

    public static (string Subject, string Body) ReservationCancelled(Reservation reservation, string reason) => (
        $"Rezervacija {reservation.ReservationNumber} je otkazana",
        Wrap($"""
            <p>Zdravo,</p>
            <p>Rezervacija broj <strong>{reservation.ReservationNumber}</strong> je otkazana.</p>
            <p>Razlog: {reason}</p>
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
