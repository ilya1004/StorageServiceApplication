import {inject, Injectable} from '@angular/core';
import {ApiService} from '../api-service';
import {Observable, of} from 'rxjs';
import {Detail} from '../../models/details/detail';
import {CreateDetailDto} from '../../models/details/createDetailDto';

@Injectable({
  providedIn: 'root'
})
export class DetailsService {
  private readonly endpoint: string = 'details';

  apiService: ApiService = inject(ApiService);

  getAll() : Observable<Detail[]> {
    return this.apiService.get<Detail[]>(this.endpoint);
    // const details: Detail[] = [
    //   {
    //     id: 1,
    //     nomenclatureCode: 'NC001',
    //     name: 'Деталь 1',
    //     count: 10,
    //     storekeeperId: 1,
    //     storekeeper: { id: 1, fullName: 'Иван Иванов' },
    //     isDeleted: false,
    //     createdAtDate: '2025-10-25',
    //     deletedAtDate: null
    //   },
    //   {
    //     id: 2,
    //     nomenclatureCode: 'NC002',
    //     name: 'Деталь 2',
    //     count: 20,
    //     storekeeperId: 2,
    //     storekeeper: { id: 2, fullName: 'Мария Петрова' },
    //     isDeleted: false,
    //     createdAtDate: '2025-10-24',
    //     deletedAtDate: null
    //   }
    // ]
    //
    // return of(details);
  }

  create(createDetailDto: CreateDetailDto) : Observable<Detail> {
    return this.apiService.post<Detail>(this.endpoint, createDetailDto);
  }

  delete(id: number) : Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
