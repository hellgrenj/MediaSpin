<template>
  <div id="app">
    <header class="masthead mb-auto">
      <div class="inner">
        <h5 class="masthead-brand">mediaspin.johanhellgren.se</h5>
        <nav class="nav nav-masthead justify-content-center">
          <router-link class="nav-link" to="About">Vad är det här?</router-link>
        </nav>
      </div>
    </header>
    <div class="main">
      <router-view />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

export default class App extends Vue {}
</script>

<style>
#app {
  /* font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px; */
}
</style>
