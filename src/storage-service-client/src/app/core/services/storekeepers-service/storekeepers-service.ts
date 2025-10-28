import {inject, Injectable} from '@angular/core';
import {ApiService} from '../api-service';
import {Observable} from 'rxjs';
import {Storekeeper} from '../../models/storekeepers/storekeeper';
import {CreateStorekeeperDto} from '../../models/storekeepers/create-storekeeper-dto';
import { HttpParams } from '@angular/common/http';
import {PaginatedResult} from '../../models/common/paginated-result';

@Injectable({
  providedIn: 'root'
})
export class StorekeepersService {
  private readonly endpoint: string = 'storekeepers';

  apiService: ApiService = inject(ApiService);

  getAll(additionalPath?: string): Observable<Storekeeper[]> {
    const fullEndpoint = additionalPath ? `${this.endpoint}/${additionalPath}` : this.endpoint;
    return this.apiService.get<Storekeeper[]>(fullEndpoint);
  }

  getPaginated(queryParams?: { [key: string]: string | number }): Observable<PaginatedResult<Storekeeper>> {
    let params: HttpParams | undefined;
    if (queryParams) {
      params = new HttpParams();
      Object.keys(queryParams).forEach(key => {
        params = params!.set(key, queryParams[key].toString());
      });
    }
    return this.apiService.get<PaginatedResult<Storekeeper>>(this.endpoint, params);
  }

  create(createStorekeeperDto: CreateStorekeeperDto) : Observable<Storekeeper> {
    return this.apiService.post<Storekeeper>(this.endpoint, createStorekeeperDto);
  }

  delete(id: number) : Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
