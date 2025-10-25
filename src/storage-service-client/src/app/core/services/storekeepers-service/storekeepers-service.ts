import {inject, Injectable} from '@angular/core';
import {ApiService} from '../api-service';
import {Observable, of} from 'rxjs';
import {CreateDetailDto} from '../../models/details/createDetailDto';
import {Storekeeper} from '../../models/storekeepers/storekeeper';
import {CreateStorekeeperDto} from '../../models/storekeepers/createStorekeeperDto';

@Injectable({
  providedIn: 'root'
})
export class StorekeepersService {
  private readonly endpoint: string = 'storekeepers';

  apiService: ApiService = inject(ApiService);

  getAll() : Observable<Storekeeper[]> {
    // return this.apiService.get<Detail[]>(this.endpoint);

    const storekeepers: Storekeeper[] = [
      { id: 1, fullName: 'Иван Иванов' },
      { id: 2, fullName: 'Мария Петрова' },
      { id: 3, fullName: 'Алексей Сидоров' }
    ];
    return of(storekeepers);
  }

  create(createStorekeeperDto: CreateStorekeeperDto) : Observable<Storekeeper> {
    return this.apiService.post<Storekeeper>(this.endpoint, createStorekeeperDto);
  }

  delete(id: number) : Observable<void> {
    return this.apiService.delete<void>(this.endpoint);
  }
}
