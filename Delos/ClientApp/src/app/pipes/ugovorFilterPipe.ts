import { Ugovor } from "../model/ugovor";
import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'ugovorFilter'
})
export class UgovorFilterPipe implements PipeTransform {
  transform(items: Ugovor[], searchText: string): any[] {
    if (!items) return [];
    if (!searchText) return items;


    searchText = searchText.toLowerCase();
    return items.filter(it => it.broj.toLowerCase()==searchText || it.kupac_naziv.toLowerCase().includes(searchText));
  }
}
