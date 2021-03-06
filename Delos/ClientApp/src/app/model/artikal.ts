import { istorija_cijena } from "./artikal - Copy";

export class Artikal {
  sifra: string;
  naziv: string;
  dobavljac: string;
  dobavljac_sifra: string;
  kolicina?: number;
  dostupnost: string;
  cijena_sa_rabatom?: number;
  cijena_prodajna?: number;
  cijena_mp?: number;
  zadnje_ucitavanje: Date;
  slike?: string[];
  vrste?: string[];
  kategorija: string;
  opis: string;
  barkod: string;
  garancija: string;
  brend: string;
  aktivan: boolean;
  istorija_cijena: Array<istorija_cijena>;
}
