import { FormMode } from "../enums/formMode";
import { jsonIgnore } from "json-ignore";
import { PonudaStavka } from "./ponudaStavka";

export class PonudaDokument {
    ponuda_broj: string;
    dokument_broj: string;
    dokument?: Blob;
    naziv?: string;
    opis?: string;
    stavka_broj?: number;
    @jsonIgnore()
    mode?: FormMode;
}

export class Dokument {
    stavka: PonudaStavka;
    naziv: string;
    dokument: string;
}
