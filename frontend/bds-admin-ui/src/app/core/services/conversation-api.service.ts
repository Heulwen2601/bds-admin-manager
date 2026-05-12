import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Conversation,
  Message,
  StartConversationRequest,
  SendMessageRequest,
  ApiResponse,
} from '../../models';

@Injectable({
  providedIn: 'root',
})
export class ConversationApiService {
  private apiUrl = `${environment.apiBaseUrl}/conversations`;

  constructor(private http: HttpClient) {}

  startConversation(request: StartConversationRequest): Observable<ApiResponse<Conversation>> {
    return this.http.post<ApiResponse<Conversation>>(this.apiUrl, request);
  }

  getConversation(id: string): Observable<ApiResponse<Conversation>> {
    return this.http.get<ApiResponse<Conversation>>(`${this.apiUrl}/${id}`);
  }

  getMessages(
    conversationId: string,
    page: number = 1,
    pageSize: number = 50,
  ): Observable<ApiResponse<Message[]>> {
    return this.http.get<ApiResponse<Message[]>>(
      `${this.apiUrl}/${conversationId}/messages?page=${page}&pageSize=${pageSize}`,
    );
  }

  sendMessage(
    conversationId: string,
    request: SendMessageRequest,
  ): Observable<ApiResponse<Message>> {
    return this.http.post<ApiResponse<Message>>(
      `${this.apiUrl}/${conversationId}/messages`,
      request,
    );
  }

  endConversation(id: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/${id}/end`, {});
  }

  // Consultant endpoints
  getAssignedConversations(): Observable<ApiResponse<Conversation[]>> {
    return this.http.get<ApiResponse<Conversation[]>>(
      `${environment.apiBaseUrl}/consultant/conversations`,
    );
  }
}
