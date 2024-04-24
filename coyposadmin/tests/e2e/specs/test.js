// https://docs.cypress.io/api/table-of-contents

beforeEach(() => {
  //uzytkownik loguje sie do aplikacji przed kazdym testem

  cy.visit("/");
  cy.wait(1000);
  cy.get("#logininput").type("123");
  cy.get("#passwordinput").type("123");
  cy.get(".btn-success").click();
  cy.get("div").contains("Dashboard").should("exist");
  cy.get("div").contains("Płatności").should("exist");
  cy.wait(200);
});
//uzytkownik dodaje edytuje wyszukuje i usuwa produkt

describe("user add edit search and remove product", () => {
  it("user add edit search and remove product", () => {
    //dodawanie
    cy.get("div").contains("Produkty").click();
    cy.wait(200);

    cy.get("div").contains("DODAJ").should("exist");
    cy.get("div").contains("DODAJ").click();
    cy.wait(200);
    cy.get(".modal-body").within(() => {
      cy.get("button").contains("Dodaj").click();
      cy.get("select").eq(0).select("pl");
      cy.get("input").eq(0).type("testowyprodukt");
      cy.get("select").eq(1).select("pl:Sypkie|en:Powder|uk:Порошок|de:Pulver");
      cy.get("input").eq(1).type("9898989898");
      cy.get("input").eq(2).type("21");
      cy.get("input").eq(3).type("21");

      cy.wait(300);
    });
    cy.get("div").contains("ZAPISZ").click();
    //wyszukiwanie
    cy.wait(300);

    cy.get("#search").type("testowyprodukt");
    cy.wait(300);
    cy.get("div").contains("testowyprodukt").should("exist");
    cy.get("div").contains("21").should("exist");
    //edycja
    cy.wait(300);
    cy.get("div").contains("EDYTUJ").click();
    cy.get("input").eq(3).type("26");
    cy.get("div").contains("ZAPISZ").click();
    cy.wait(300);
    cy.get("div").contains("26").should("exist");
    //usuniecie
    cy.get("div").contains("USUŃ").click();
    cy.get("#deleteModal").within(() => {
      cy.get("div").contains("USUŃ").click();
    });
    cy.wait(300);

    cy.get(".product").should("not.exist");
    cy.get(".product").should("not.exist");
  });
});

//uzytkownik dodaje edytuje i usuwa kategorie
describe("user add edit and remove category", () => {
  it("user add edit and remove category", () => {
    //dodawanie
    cy.get("div").contains("Kategorie").click();
    cy.wait(200);

    cy.get("div").contains("DODAJ").should("exist");
    cy.get("div").contains("DODAJ").click();
    cy.wait(200);
    cy.get(".modal-body").within(() => {
      cy.get("button").contains("Dodaj").click();
      cy.get("select").eq(0).select("pl");
      cy.get("input").eq(0).type("testowakategoria");
      cy.wait(300);
    });
    cy.get("div").contains("ZAPISZ").click();
    //edycja

    cy.get("a").contains("100").click();
    cy.wait(300);
    cy.get("div").contains("testowakategoria").should("exist");
    cy.get("div").contains("testowakategoria").next().next().next().click();
    cy.wait(200);

    cy.get("input").eq(0).type("13");
    cy.wait(200);

    cy.get("div").contains("ZAPISZ").click();
    cy.wait(300);
    cy.get("div").contains("testowakategoria13").should("exist");
    //usuniecie
    cy.get("div")
      .contains("testowakategoria")
      .next()
      .next()
      .next()
      .next()
      .click();

    cy.get("#deleteModal").within(() => {
      cy.get("div").contains("USUŃ").click();
    });
    cy.wait(300);

    cy.get(".category").contains("testowakategoria13").should("not.exist");
  });
});

//uzytkownik dodaje edytuje i usuwa uzytkownika

describe("user add edit and remove user", () => {
  it("user add edit and remove user", () => {
    //dodawanie
    cy.get("div").contains("Użytkownicy").click();
    cy.wait(200);

    cy.get("div").contains("DODAJ").should("exist");
    cy.get("div").contains("DODAJ").click();
    cy.wait(200);
    cy.get(".modal-body").within(() => {
      cy.wait(300);
      cy.get("input").eq(0).type("testowyuser");
      cy.wait(100);
      cy.get("input").eq(4).type("testowy@user.pl");
      cy.wait(100);
      cy.get("input").eq(5).type("12345678");

      cy.wait(300);
    });
    cy.get("div").contains("ZAPISZ").click();
    //edycja

    cy.get("a").contains("100").click();
    cy.wait(300);
    cy.get("div").contains("testowyuser").should("exist");
    cy.get("div").contains("testowy@user.pl").next().next().click();
    cy.get("input").eq(0).type("13");
    cy.wait(200);

    cy.get("div").contains("ZAPISZ").click();
    cy.wait(300);
    cy.get("div").contains("testowyuser13").should("exist");
    //usuniecie
    cy.get("div").contains("testowy@user.pl").next().next().next().click();

    cy.get("#deleteModal").within(() => {
      cy.get("div").contains("USUŃ").click();
    });
    cy.wait(300);

    cy.get(".user").contains("testowyuser13").should("not.exist");
  });
});
//uzytkownik dodaje edytuje i usuwa pracownika
describe("user add edit and remove employee", () => {
  it("user add edit and remove employee", () => {
    //dodawanie
    cy.get("div").contains("Pracownicy").click();
    cy.wait(200);

    cy.get("div").contains("DODAJ").should("exist");
    cy.get("div").contains("DODAJ").click();
    cy.wait(200);
    cy.get(".modal-body").within(() => {
      cy.wait(300);
      cy.get("input").eq(0).type("testowypracownik");
      cy.wait(100);
      cy.get("input").eq(1).type("12345678");
      cy.wait(100);
      cy.get("input").eq(2).type("1234");

      cy.wait(300);
    });
    cy.get("div").contains("ZAPISZ").click();
    //edycja

    cy.get("a").contains("100").click();
    cy.wait(300);
    cy.get("div").contains("testowypracownik").should("exist");
    cy.get("div")
      .contains("testowypracownik")
      .next()
      .next()
      .next()
      .next()
      .click();
    cy.get("input").eq(0).type("13");
    cy.wait(200);

    cy.get("div").contains("ZAPISZ").click();
    cy.wait(300);
    cy.get("div").contains("testowypracownik13").should("exist");
    //usuniecie
    cy.get("div")
      .contains("testowypracownik13")
      .next()
      .next()
      .next()
      .next()
      .next()
      .click();

    cy.get("#deleteModal").within(() => {
      cy.get("div").contains("USUŃ").click();
    });
    cy.wait(300);

    cy.get(".employee").contains("testowypracownik13").should("not.exist");
  });
});
//uzytkownik dodaje edytuje i usuwa platnosc
describe("user add edit and remove paymethod", () => {
  it("user add edit and remove paymethod", () => {
    //dodawanie
    cy.get("div").contains("Płatności").click();
    cy.wait(200);

    cy.get("div").contains("DODAJ").should("exist");
    cy.get("div").contains("DODAJ").click();
    cy.wait(200);
    cy.get(".modal-body").within(() => {
      cy.wait(300);
      cy.get("input").eq(0).type("testowaplatnosc");

      cy.wait(300);
    });
    cy.get("div").contains("ZAPISZ").click();
    //edycja

    cy.get("a").contains("100").click();
    cy.wait(300);
    cy.get("div").contains("testowaplatnosc").should("exist");
    cy.get("div").contains("testowaplatnosc").next().next().next().click();
    cy.get("input").eq(0).type("13");
    cy.wait(200);

    cy.get("div").contains("ZAPISZ").click();
    cy.wait(300);
    cy.get("div").contains("testowaplatnosc13").should("exist");
    //usuniecie
    cy.get("div")
      .contains("testowaplatnosc13")
      .next()
      .next()
      .next()
      .next()
      .click();

    cy.get("#deleteModal").within(() => {
      cy.get("div").contains("USUŃ").click();
    });
    cy.wait(300);

    cy.get(".payment_method").contains("testowaplatnosc13").should("not.exist");
  });
});
//uzytkownik dodaje edytuje i usuwa jezyk

//uzytkownik wylogowuje sie
describe("user logout", () => {
  it("user logout", () => {
    cy.get("div").contains("Wyloguj").click();
    cy.wait(200);

    cy.get("label").contains("Login").should("exist");
  });
});

//todo uzytkownik instaluje kase
