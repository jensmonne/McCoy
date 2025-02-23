import math

LEVEL_UP_EMOJIS = ["😘", "🫃", "💪", "😼", "👹", "💅", "👻"]
LEVEL_SHOW_EMOJIS = ["😹", "😮", "🙏", "👺", "🙄", "🤓", "🙀"]

# function to calculate XP required for next level (Exponantial growth)
def required_level_xp(level):
    return math.floor(100 * (1.5 ** (level - 1)))