import { partner } from "./partner";
import { Korisnik } from "./korisnik";

export class Prijava {
  broj: string;
  broj_naloga: string;
  datum?: Date;
  partner_dobavljac: partner;
  partner?: partner;
  korisnik?: Korisnik;
  kupac_sifra?: number;
  kupac_ime: string;
  kupac_adresa: string;
  kupac_telefon: string;
  kupac_email: string;
  model: string;
  serijski_broj: string;
  dodatna_oprema: string;
  predmet: string;
  napomena_servisera: string;
  serviser: string;
  serviser_primio: string;
  datum_vracanja?: Date;
  poslat_mejl_dobavljacu?: Date;
  dobavljac_sifra?: number;
  dobavljac: string;
  garantni_rok?: number;
  broj_garantnog_lista: string;
  broj_racuna: string;
  instalacija_os: boolean;
  instalacija_office: boolean;
  instalacija_ostalo: boolean;
  instalacija: string;

  selected: boolean;
}
