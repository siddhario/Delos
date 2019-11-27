import { jsonIgnore } from "json-ignore";
import { FormMode } from "../enums/formMode";

export class PonudaStavka {
    ponuda_broj: string;
    stavka_broj: number;
    artikal_naziv: string;
    opis: string;
    jedinica_mjere: string;
    kolicina: number = null;
    cijena_bez_pdv: number;
    cijena_bez_pdv_sa_rabatom: number;
    rabat_procenat: number;
    rabat_iznos: number;
    iznos_bez_pdv: number;
    iznos_bez_pdv_sa_rabatom: number;
    cijena_nabavna: number;
    vrijednost_nabavna: number;
    marza_procenat: number;
    ruc: number;
    pdv_stopa: number;
    pdv: number;
    cijena_sa_pdv: number;
    iznos_sa_pdv: number;
    napomena: string;
    @jsonIgnore()
    mode?: FormMode = FormMode.View;
}
