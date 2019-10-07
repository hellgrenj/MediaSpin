import Vue from 'vue'
import Router from 'vue-router'
import Start from '@/components/Start.vue';
import About from '@/components/About.vue';

Vue.use(Router)

export default new Router({
  routes: [{
    path: '/Start',
    name: 'Start',
    component: Start
  },
  {
    path: '/',
    name: 'Start',
    component: Start
  },
  {
    path: '/About',
    name: 'About',
    component: About

  }
  ]
})
