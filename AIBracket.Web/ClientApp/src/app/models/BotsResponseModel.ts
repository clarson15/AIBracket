import { BotHistoryResponseModel } from "./BotHistoryResponseModel";

export class BotsResponseModel {
  id: string;
  name: string;
  game: number;
  privateKey: string;
  showSecret: boolean;
  history: BotHistoryResponseModel[];
}
