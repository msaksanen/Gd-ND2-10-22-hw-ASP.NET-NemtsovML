import { Doctorfulldata } from './doctorfulldata';
import { Filterspecviewmodel } from './filterspecviewmodel';
import { Pageviewmodel } from './pageviewmodel';
import { Sortviewmodel } from './sortviewmodel';

export class Doctdataindexviewmodel {
 users?: Doctorfulldata[];
 pageViewModel?: Pageviewmodel;
 filterViewModel?: Filterspecviewmodel;
 sortViewModel?: Sortviewmodel;
 processOptions?: string;
 reflink?: string;
}
