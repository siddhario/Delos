import { jsonIgnore } from "json-ignore";
import { FormMode } from "../enums/formMode";

export class UgovorRata {
  ugovorbroj: string;
  broj_rate: number;
  rok_placanja: Date;
  datum_placanja: Date;
  iznos: number;
  uplaceno: number;
  napomena: string;
  @jsonIgnore()
  formMode: FormMode = FormMode.View;
}
