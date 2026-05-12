import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Message, Conversation } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection | null = null;
  private messagesSubject = new BehaviorSubject<{[conversationId: string]: Message[]}>({});
  private conversationsSubject = new BehaviorSubject<Conversation[]>([]);

  public messages$ = this.messagesSubject.asObservable();
  public conversations$ = this.conversationsSubject.asObservable();

  constructor() {}

  startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.chatHubUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Chat hub connection started');
        this.registerHandlers();
      })
      .catch(err => console.error('Error starting chat hub:', err));
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      const currentMessages = this.messagesSubject.value;
      const conversationMessages = currentMessages[message.conversationId] || [];
      const updatedMessages = {
        ...currentMessages,
        [message.conversationId]: [...conversationMessages, message]
      };
      this.messagesSubject.next(updatedMessages);
    });

    this.hubConnection.on('ConversationAssigned', (conversation: Conversation) => {
      const currentConversations = this.conversationsSubject.value;
      this.conversationsSubject.next([...currentConversations, conversation]);
    });

    this.hubConnection.on('ConversationEnded', (conversationId: string) => {
      const currentConversations = this.conversationsSubject.value;
      const updatedConversations = currentConversations.map(conv =>
        conv.id === conversationId ? { ...conv, status: 'Closed' as const } : conv
      );
      this.conversationsSubject.next(updatedConversations);
    });
  }

  joinConversation(conversationId: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('JoinConversation', conversationId);
    }
  }

  leaveConversation(conversationId: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveConversation', conversationId);
    }
  }

  updateMessages(conversationId: string, messages: Message[]): void {
    const currentMessages = this.messagesSubject.value;
    this.messagesSubject.next({
      ...currentMessages,
      [conversationId]: messages
    });
  }

  updateConversations(conversations: Conversation[]): void {
    this.conversationsSubject.next(conversations);
  }

  getMessagesForConversation(conversationId: string): Message[] {
    return this.messagesSubject.value[conversationId] || [];
  }
}