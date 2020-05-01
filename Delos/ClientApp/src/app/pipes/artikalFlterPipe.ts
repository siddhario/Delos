import { Pipe, PipeTransform } from "@angular/core";
import { Artikal } from "../model/artikal";

@Pipe({
    name: 'artikalFilter'
})
export class ArtikalFlterPipe implements PipeTransform {
    transform(items: Artikal[], searchText: string): any[] {
        if (!items) return [];
        if (!searchText || searchText.length < 3) return items;
        let words = searchText.split(" ");
        let exp = "";
        for (let i = 0; i < words.length; i++) {
            exp += "(?=.*" + words[i].toLowerCase() + ".*)";
        }
        exp += ".+";
       
        searchText = searchText.toLowerCase();
        return items.filter(it => it.naziv.toLowerCase().match(exp)!=null);
    }
}

