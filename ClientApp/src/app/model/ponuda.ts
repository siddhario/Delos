import { jsonIgnore } from "json-ignore";
import { Korisnik } from "./korisnik";
import { partner } from "./partner";
import { PonudaStavka } from "./ponudaStavka";
import { PonudaDokument } from "./ponudaDokument";

export class Ponuda {
    broj: string;
    datum: Date;
    selected: boolean;
    partner: partner;
    partner_sifra: number;
    partner_naziv: string;
    partner_adresa: string;
    partner_telefon: string;
    partner_email: string;
    iznos_sa_rabatom: number;
    partner_jib: string;
    rok_vazenja: string;
    rok_isporuke: string;
    paritet: string;
    paritet_kod: string;
    valuta_placanja: string;
    pdv: number;
    iznos_sa_pdv: number;
    predmet: string;
    status: string;
    radnik: string;
    @jsonIgnore()
    stavke: PonudaStavka[];
    korisnik: Korisnik;
    rabat: number;
    iznos_bez_rabata: number;
    @jsonIgnore()
    dokumenti: PonudaDokument[];
}
