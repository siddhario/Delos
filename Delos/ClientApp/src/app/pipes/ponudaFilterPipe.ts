import { Ponuda } from "../model/ponuda";
import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'ponudaFilter'
})
export class PonudaFilterPipe implements PipeTransform {
    transform(items: Ponuda[], searchText: string): any[] {
        if (!items) return [];
        if (!searchText) return items;
        searchText = searchText.toLowerCase();
        return items.filter(it => {
            return it.partner_naziv.toLowerCase().includes(searchText.toLowerCase()) || it.broj.includes(searchText)
                || (!!it.stavke &&
                    it.stavke.filter(s =>
                        (!!s.artikal_naziv && s.artikal_naziv.toLowerCase().includes(searchText))
                        ||
                        (!!s.opis && s.opis.toLowerCase().includes(searchText))
                    ).length > 0);
        });
    }
}
