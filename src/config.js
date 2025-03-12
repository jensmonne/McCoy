import dotenv from "dotenv";

dotenv.config();

export const LEVEL_UP_EMOJIS = ["😘", "🫃", "💪", "😼", "👹", "💅", "👻"]
export const LEVEL_SHOW_EMOJIS = ["😹", "😮", "🙏", "👺", "🙄", "🤓", "🙀"]

export function IsAdminOrOwner(member) {
    return member.permissions.has("ADMINISTRATOR") || member.id === process.env.ADMIN_ID;
}

export function RequiredLevelXp(level) {
    return Math.floor((5 * (level ** 2) + (50 * level) + 100) / 2);
}