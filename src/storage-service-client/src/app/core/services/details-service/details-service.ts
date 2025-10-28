import {inject, Injectable} from '@angular/core';
import {ApiService} from '../api-service';
import {Observable} from 'rxjs';
import {Detail} from '../../models/details/detail';
import {CreateDetailDto} from '../../models/details/create-detail-dto';
import {HttpParams} from '@angular/common/http';
import {PaginatedResult} from '../../models/common/paginated-result';

@Injectable({
  providedIn: 'root'
})
export class DetailsService {
  private readonly endpoint: string = 'details';

  apiService: ApiService = inject(ApiService);

  getPaginated(queryParams?: { [key: string]: string | number }): Observable<PaginatedResult<Detail>> {
    let params: HttpParams | undefined;
    if (queryParams) {
      params = new HttpParams();
      Object.keys(queryParams).forEach(key => {
        params = params!.set(key, queryParams[key].toString());
      });
    }
    return this.apiService.get<PaginatedResult<Detail>>(this.endpoint, params);
  }

  create(createDetailDto: CreateDetailDto) : Observable<Detail> {
    return this.apiService.post<Detail>(this.endpoint, createDetailDto);
  }

  delete(id: number) : Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
