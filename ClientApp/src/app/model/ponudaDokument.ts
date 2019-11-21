import { FormMode } from "../enums/formMode";

export class PonudaDokument {
    ponuda_broj: string;
    dokument_broj: string;
    dokument?: Blob;
    naziv?: string;
    opis?: string;
    mode?: FormMode;
}
