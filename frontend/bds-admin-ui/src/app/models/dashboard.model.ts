export interface AdminDashboard {
  totalProperties: number;
  pendingProperties: number;
  approvedProperties: number;
  rejectedProperties: number;
  totalUsers: number;
  totalConsultants: number;
  totalConversations: number;
  activeConversations: number;
}

export interface SellerDashboard {
  properties: number;
  pendingProperties: number;
  leads: number;
  conversations: number;
}

export interface ConsultantPerformance {
  consultantId: string;
  consultantName: string;
  totalConversations: number;
  activeConversations: number;
  closedConversations: number;
  averageResponseTime: number;
}
