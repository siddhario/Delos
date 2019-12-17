import { FormMode } from "../enums/formMode";
import { jsonIgnore } from "json-ignore";

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
