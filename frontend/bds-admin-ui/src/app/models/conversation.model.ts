export interface Conversation {
  id: string;
  userId?: string;
  consultantId?: string;
  guestName?: string;
  guestPhone?: string;
  status: 'Active' | 'Closed';
  createdAt: string;
  updatedAt: string;
}

export interface Message {
  id: string;
  conversationId: string;
  senderId?: string;
  senderName: string;
  content: string;
  sentAt: string;
  isFromUser: boolean;
}

export interface StartConversationRequest {
  guestName?: string;
  guestPhone?: string;
  initialMessage: string;
}

export interface SendMessageRequest {
  content: string;
}