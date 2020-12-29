import { Korisnik } from "./korisnik";
import { partner } from "./partner";
import { UgovorRata } from "./ugovorRata";

export class Ugovor {
  broj?: string;
  datum?: Date;
  kupac_sifra?: number;
  kupac_maticni_broj?: string;
  kupac_broj_lk?: string;
  kupac_naziv?: string;
  kupac_adresa?: string;
  kupac_telefon?: string;
  broj_racuna?: string;
  radnik?: string;
  inicijalno_placeno?: number;
  iznos_bez_pdv?: number;
  pdv?: number;
  iznos_sa_pdv?: number;
  broj_rata?: number;
  suma_uplata?: number;
  preostalo_za_uplatu?: number;
  status?: string;
  napomena?: string;
  mk?: boolean;
  uplaceno_po_ratama?: number;
  korisnik?: Korisnik;
  partner?: partner;
  rate?: UgovorRata[];
  odobrio?: string;
}
