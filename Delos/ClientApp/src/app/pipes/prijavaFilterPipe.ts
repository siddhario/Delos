import { Prijava } from "../model/prijava";
import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'prijavaFilter'
})
export class PrijavaFilterPipe implements PipeTransform {
  transform(items: Prijava[], searchText: string): any[] {
    if (!items) return [];
    if (!searchText) return items;


    searchText = searchText.toLowerCase();
    return items.filter(it => it.broj.toLowerCase()==searchText || it.kupac_ime.toLowerCase().includes(searchText) || (it.predmet != null && it.predmet.toLowerCase().includes(searchText)));
  }
}
