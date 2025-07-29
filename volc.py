import pygame
import sys
import random
import math

# Initialize Pygame
pygame.init()

# Screen dimensions
WIDTH, HEIGHT = 800, 600
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Volcano Eruption Simulation")

# Colors
BLACK = (0, 0, 0)
BROWN = (139, 69, 19)
ORANGE = (255, 165, 0)
RED = (255, 0, 0)
SKY_BLUE = (135, 206, 235)

# Volcano parameters
VOLCANO_BASE = WIDTH // 2
VOLCANO_HEIGHT = HEIGHT // 2
VOLCANO_PEAK = (VOLCANO_BASE, HEIGHT - VOLCANO_HEIGHT)

# Particle class
class Particle:
    def __init__(self, x, y):
        self.x = x
        self.y = y
        self.vx = random.uniform(-5, 5)  # Horizontal velocity
        self.vy = random.uniform(-15, -10)  # Upward velocity
        self.size = random.randint(2, 5)
        self.color = random.choice([ORANGE, RED])
        self.gravity = 0.2  # Gravity strength
        self.lifespan = random.randint(100, 200)  # Frames to live
        self.age = 0

    def update(self):
        self.vy += self.gravity  # Apply gravity
        self.x += self.vx
        self.y += self.vy
        self.age += 1

    def draw(self, screen):
        if self.age < self.lifespan:
            pygame.draw.circle(screen, self.color, (int(self.x), int(self.y)), self.size)

# List to hold particles
particles = []

# Main loop
running = True
clock = pygame.time.Clock()

while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    # Fill background
    screen.fill(SKY_BLUE)

    # Draw volcano (simple triangle)
    pygame.draw.polygon(screen, BROWN, [(VOLCANO_BASE - 200, HEIGHT), (VOLCANO_BASE + 200, HEIGHT), VOLCANO_PEAK])

    # Erupt particles randomly
    if random.random() < 0.3:  # Chance to erupt per frame
        for _ in range(5):  # Number of particles per eruption
            particles.append(Particle(VOLCANO_PEAK[0], VOLCANO_PEAK[1]))

    # Update and draw particles
    for particle in particles[:]:
        particle.update()
        particle.draw(screen)
        if particle.age >= particle.lifespan or particle.y > HEIGHT:
            particles.remove(particle)

    pygame.display.flip()
    clock.tick(60)

pygame.quit()
sys.exit()