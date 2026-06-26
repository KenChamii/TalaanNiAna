import {
  Component,
  OnDestroy,
  AfterViewInit,
} from '@angular/core';

interface Particle {
  x: number;
  y: number;
  vx: number;
  vy: number;
  radius: number;
  alpha: number;
  fadeSpeed: number;
}

@Component({
  selector: 'app-particles',
  standalone: true,
  template: ``,
})
export class ParticlesComponent implements AfterViewInit, OnDestroy {
  private canvas!: HTMLCanvasElement;
  private ctx!: CanvasRenderingContext2D;
  private particles: Particle[] = [];
  private animId = 0;

  private readonly COUNT = 80;
  private readonly MAX_DIST = 140;
  private readonly COLOR = '99, 141, 245';

  ngAfterViewInit() {
    this.canvas = document.createElement('canvas');
    this.canvas.id = 'particles-canvas';
    this.canvas.style.cssText = `
      position: fixed !important;
      top: 0 !important;
      left: 0 !important;
      width: 100vw !important;
      height: 100vh !important;
      pointer-events: none !important;
      z-index: 0 !important;
      display: block !important;
    `;

    // Set actual drawing buffer size
    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;

    document.body.appendChild(this.canvas);
    this.ctx = this.canvas.getContext('2d')!;

    this.spawn();
    this.loop();

    window.addEventListener('resize', this.onResize);
  }

  ngOnDestroy() {
    cancelAnimationFrame(this.animId);
    window.removeEventListener('resize', this.onResize);
    const el = document.getElementById('particles-canvas');
    if (el) el.remove();
  }

  private onResize = () => {
    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;
    this.spawn();
  };

  private spawn() {
    const w = this.canvas.width;
    const h = this.canvas.height;
    this.particles = Array.from({ length: this.COUNT }, () => ({
      x: Math.random() * w,
      y: Math.random() * h,
      vx: (Math.random() - 0.5) * 0.5,
      vy: (Math.random() - 0.5) * 0.5,
      radius: Math.random() * 2 + 0.8,
      alpha: Math.random() * 0.5 + 0.2,
      fadeSpeed: (Math.random() * 0.004 + 0.001) * (Math.random() < 0.5 ? 1 : -1),
    }));
  }

  private loop() {
    this.animId = requestAnimationFrame(() => this.loop());
    const w = this.canvas.width;
    const h = this.canvas.height;
    this.ctx.clearRect(0, 0, w, h);

    for (const p of this.particles) {
      p.x += p.vx;
      p.y += p.vy;
      if (p.x < 0) p.x = w;
      if (p.x > w) p.x = 0;
      if (p.y < 0) p.y = h;
      if (p.y > h) p.y = 0;

      p.alpha += p.fadeSpeed;
      if (p.alpha > 0.7 || p.alpha < 0.1) p.fadeSpeed *= -1;

      this.ctx.beginPath();
      this.ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
      this.ctx.fillStyle = `rgba(${this.COLOR}, ${p.alpha})`;
      this.ctx.fill();
    }

    for (let i = 0; i < this.particles.length; i++) {
      for (let j = i + 1; j < this.particles.length; j++) {
        const a = this.particles[i];
        const b = this.particles[j];
        const dx = a.x - b.x;
        const dy = a.y - b.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        if (dist < this.MAX_DIST) {
          const lineAlpha = (1 - dist / this.MAX_DIST) * 0.15;
          this.ctx.beginPath();
          this.ctx.moveTo(a.x, a.y);
          this.ctx.lineTo(b.x, b.y);
          this.ctx.strokeStyle = `rgba(${this.COLOR}, ${lineAlpha})`;
          this.ctx.lineWidth = 0.8;
          this.ctx.stroke();
        }
      }
    }
  }
}
