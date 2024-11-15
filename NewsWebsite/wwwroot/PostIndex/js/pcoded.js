// 'use strict';
var flg = '0';

// Function to handle menu click events (collpase menus and it's submenu also collapse)
if (!main_layout_change('horizontal')) {
  function menu_click() {
    // Remove click event listeners from navigation menu items
    var elem = document.querySelectorAll('.pc-navbar li');
    for (var j = 0; j < elem.length; j++) {
      elem[j].removeEventListener('click', function () {});
    }

    // Hide submenu items (when menu link not active then submenu hide)
    var elem = document.querySelectorAll('.pc-navbar li:not(.pc-trigger) .pc-submenu');
    for (var j = 0; j < elem.length; j++) {
      elem[j].style.display = 'none';
    }

    // Add click event listeners to main menu items (for first menu level collapse)
    var pc_link_click = document.querySelectorAll('.pc-navbar > li:not(.pc-caption).pc-hasmenu');
    for (var i = 0; i < pc_link_click.length; i++) {
      pc_link_click[i].addEventListener('click', function (event) {
        // Prevent parent elements from triggering their events
        event.stopPropagation();
        var targetElement = event.target;
        if (targetElement.tagName == 'SPAN') {
          targetElement = targetElement.parentNode;
        }
        // Toggle submenu visibility (active remove who has menu link not clicked and it's submenu also hide)
        if (targetElement.parentNode.classList.contains('pc-trigger')) {
          targetElement.parentNode.classList.remove('pc-trigger');
          slideUp(targetElement.parentNode.children[1], 200);
          window.setTimeout(() => {
            targetElement.parentNode.children[1].removeAttribute('style');
            targetElement.parentNode.children[1].style.display = 'none';
          }, 200);
        } else {
          // Close other open submenus
          var tc = document.querySelectorAll('li.pc-trigger');
          for (var t = 0; t < tc.length; t++) {
            var c = tc[t];
            c.classList.remove('pc-trigger');
            slideUp(c.children[1], 200);
            window.setTimeout(() => {
              c.children[1].removeAttribute('style');
              c.children[1].style.display = 'none';
            }, 200);
          }

          // Open clicked submenu (for active menu link)
          targetElement.parentNode.classList.add('pc-trigger');
          var submenu_list = targetElement.children[1];
          if (submenu_list) {
            slideDown(targetElement.parentNode.children[1], 200);
          }
        }
      });
    }

    // Initialize SimpleBar for navbar content if available
    if (document.querySelector('.navbar-content')) {
      new SimpleBar(document.querySelector('.navbar-content'));
    }

    // Add click event listeners to submenu items
    var pc_sub_link_click = document.querySelectorAll('.pc-navbar > li:not(.pc-caption) li.pc-hasmenu');
    for (var i = 0; i < pc_sub_link_click.length; i++) {
      pc_sub_link_click[i].addEventListener('click', function (event) {
        var targetElement = event.target;
        if (targetElement.tagName == 'SPAN') {
          targetElement = targetElement.parentNode;
        }
        event.stopPropagation();
        // Toggle submenu visibility
        if (targetElement.parentNode.classList.contains('pc-trigger')) {
          targetElement.parentNode.classList.remove('pc-trigger');
          slideUp(targetElement.parentNode.children[1], 200);
        } else {
          // Close other open submenus
          var tc = targetElement.parentNode.parentNode.children;
          for (var t = 0; t < tc.length; t++) {
            var c = tc[t];
            c.classList.remove('pc-trigger');
            if (c.tagName == 'LI') {
              c = c.children[0];
            }
            if (c.parentNode.classList.contains('pc-hasmenu')) {
              slideUp(c.parentNode.children[1], 200);
            }
          }

          // Open clicked submenu
          targetElement.parentNode.classList.add('pc-trigger');
          var submenu_list = targetElement.parentNode.children[1];
          if (submenu_list) {
            submenu_list.removeAttribute('style');
            slideDown(submenu_list, 200);
          }
        }
      });
    }
  }
}

// Initialize menu click function on DOMContentLoaded (function call on page load)
document.addEventListener('DOMContentLoaded', menu_click);

// Initialize various components on DOMContentLoaded
document.addEventListener('DOMContentLoaded', function () {
  // feather icon start
  feather.replace();
  // feather icon end

  // Check for specific layout and add scrollbar if necessary(add scrollbar from 1024 screen size in horizontal layout)
  if (document.querySelector('body').hasAttribute('data-pc-layout')) {
    if (document.querySelector('body').getAttribute('data-pc-layout') == 'horizontal') {
      var docW = window.innerWidth;
      if (docW <= 1024) {
        add_scroller();
      }
    }
  } else {
    add_scroller();
  }

  // Menu collapse click start (when click it's open and close sidebar for mobile screen)
  var mobile_collapse_over = document.querySelector('#mobile-collapse');
  if (mobile_collapse_over) {
    mobile_collapse_over.addEventListener('click', function () {
      var mobile_sidebar = document.querySelector('.pc-sidebar');
      if (mobile_sidebar) {
        if (document.querySelector('.pc-sidebar').classList.contains('mob-sidebar-active')) {
          rm_menu();
        } else {
          document.querySelector('.pc-sidebar').classList.add('mob-sidebar-active');
          // add overlay when sidebar open
          document.querySelector('.pc-sidebar').insertAdjacentHTML('beforeend', '<div class="pc-menu-overlay"></div>');
          document.querySelector('.pc-menu-overlay').addEventListener('click', function () {
            rm_menu();
          });
        }
      }
    });
  }
  // Menu collapse click end

  // header dropdown scrollbar start
  if (document.querySelector('.header-notification-scroll')) {
    new SimpleBar(document.querySelector('.header-notification-scroll'));
  }

  if (document.querySelector('.profile-notification-scroll')) {
    new SimpleBar(document.querySelector('.profile-notification-scroll'));
  }
  // header dropdown scrollbar end

  // component scrollbar start
  if (document.querySelector('.component-list-card .card-body')) {
    new SimpleBar(document.querySelector('.component-list-card .card-body'));
  }
  // component- dropdown scrollbar end

  // for sidebar close
  var sidebar_hide = document.querySelector('#sidebar-hide');
  if (sidebar_hide) {
    sidebar_hide.addEventListener('click', function () {
      if (document.querySelector('.pc-sidebar').classList.contains('pc-sidebar-hide')) {
        document.querySelector('.pc-sidebar').classList.remove('pc-sidebar-hide');
      } else {
        document.querySelector('.pc-sidebar').classList.add('pc-sidebar-hide');
      }
    });
  }

  // for input focus add when click search icon
  if (document.querySelector('.trig-drp-search')) {
    const search_drp = document.querySelector('.trig-drp-search');
    search_drp.addEventListener('shown.bs.dropdown', (event) => {
      document.querySelector('.drp-search input').focus();
    });
  }

  // layout options (when click customizer layout options according to that set value in local storage)
  setLayout();
  var if_layout = document.querySelectorAll('.theme-main-layout');
  var layoutValue = 'vertical';
  if (if_layout) {
    var preset_layout = document.querySelectorAll('.theme-main-layout > a');
    preset_layout.forEach(function (element) {
      element.addEventListener('click', function () {
        // Reload the page after setting the layout for the first time to sync in all open tabs
        location.reload();

        document.querySelectorAll('.theme-main-layout > a').forEach(function (el) {
          el.classList.remove('active');
        });
        this.classList.add('active');
        if (this.getAttribute('data-value') == 'horizontal') {
          layoutValue = 'horizontal';
        } else if (this.getAttribute('data-value') == 'compact') {
          layoutValue = 'compact';
        } else if (this.getAttribute('data-value') == 'tab') {
          layoutValue = 'tab';
        } else if (this.getAttribute('data-value') == 'color-header') {
          layoutValue = 'color-header';
        } else {
          layoutValue = 'vertical';
        }

        // Set data to localStorage
        localStorage.setItem('layout', layoutValue);

        setLayout();
      });
    });
  }
});

// Function to set the layout based on data stored in localStorage
function setLayout() {
  var layout = localStorage.getItem('layout'); // Retrieve layout data from localStorage

  // Pass the layout value to main_layout_change function
  main_layout_change(layout);

  // Load corresponding scripts or perform actions based on the layout value
  if (layout !== null && layout !== '') {
    var script = document.createElement('script');
    if (layout === 'horizontal') {
      document.querySelector('.pc-sidebar').classList.add('d-none');
      script.src = '../assets/js/layout-horizontal.js'; // Load script for horizontal layout
      document.body.appendChild(script);
    } else if (layout === 'color-header') {
      // Change logo color for color-header layout
      if (document.querySelector('.pc-sidebar .m-header .logo-lg')) {
        document.querySelector('.pc-sidebar .m-header .logo-lg').setAttribute('src', '../assets/images/logo-white.svg');
      }
    } else if (layout === 'compact') {
      script.src = '../assets/js/layout-compact.js'; // Load script for compact layout
      document.body.appendChild(script);
    } else if (layout === 'tab') {
      script.src = '../assets/js/layout-tab.js'; // Load script for tab layout
      document.body.appendChild(script);
    }
  }

  // If no layout data found in localStorage, set default layout to 'vertical'
  if (layout === null) {
    main_layout_change('vertical');
    localStorage.setItem('layout', 'vertical');
  }
}

// Function to handle menu click and scrollbar initialization
function add_scroller() {
  menu_click(); // Call menu_click function
  // Initialize scrollbar if navbar-content exists
  if (document.querySelector('.navbar-content')) {
    new SimpleBar(document.querySelector('.navbar-content'));
  }
}

// Function to hide mobile menu (sidebar hide on click overlay)
function rm_menu() {
  // Remove active class from mobile menu elements
  var menu_list = document.querySelector('.pc-sidebar');
  if (menu_list) {
    document.querySelector('.pc-sidebar').classList.remove('mob-sidebar-active');
  }
  if (document.querySelector('.topbar')) {
    document.querySelector('.topbar').classList.remove('mob-sidebar-active');
  }

  // Remove menu overlay elements
  document.querySelector('.pc-sidebar .pc-menu-overlay').remove();
  if (document.querySelector('.topbar .pc-menu-overlay')) {
    document.querySelector('.topbar .pc-menu-overlay').remove();
  }
}

// Function to remove overlay menu
function remove_overlay_menu() {
  // Remove active class and overlay elements
  document.querySelector('.pc-sidebar').classList.remove('pc-over-menu-active');
  if (document.querySelector('.topbar')) {
    document.querySelector('.topbar').classList.remove('mob-sidebar-active');
  }
  document.querySelector('.pc-sidebar .pc-menu-overlay').remove();
  document.querySelector('.topbar .pc-menu-overlay').remove();
}

// Event listener to initialize tooltips, popovers, and toasts on window load
window.addEventListener('load', function () {
  // Initialize tooltips
  var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Initialize popovers
  var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
  var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
  });

  // Initialize toasts
  var toastElList = [].slice.call(document.querySelectorAll('.toast'));
  var toastList = toastElList.map(function (toastEl) {
    return new bootstrap.Toast(toastEl);
  });

  // Remove pre-loader after page load
  var loader = document.querySelector('.loader-bg');
  if (loader) {
    loader.remove();
  }
});

// Function to mark active menu items based on current page URL
var elem = document.querySelectorAll('.pc-sidebar .pc-navbar a');
for (var l = 0; l < elem.length; l++) {
  // Check if current URL matches menu item URL
  var pageUrl = window.location.href.split(/[?#]/)[0];
  if (elem[l].href == pageUrl && elem[l].getAttribute('href') != '') {
    // Add active class to matching menu item and its parent elements
    elem[l].parentNode.classList.add('active');
    elem[l].parentNode.parentNode.parentNode.classList.add('pc-trigger');
    elem[l].parentNode.parentNode.parentNode.classList.add('active');
    elem[l].parentNode.parentNode.style.display = 'block';
    elem[l].parentNode.parentNode.parentNode.parentNode.parentNode.classList.add('pc-trigger');
    elem[l].parentNode.parentNode.parentNode.parentNode.style.display = 'block';
  }
}

// Event listener to handle like events (added to favourites)
var tc = document.querySelectorAll('.prod-likes .form-check-input');
for (var t = 0; t < tc.length; t++) {
  var prod_like = tc[t];
  // Add event listener to like checkboxes
  prod_like.addEventListener('change', function (event) {
    if (event.currentTarget.checked) {
      // Add like animation if checkbox is checked
      prod_like = event.target;
      prod_like.parentNode.insertAdjacentHTML(
        'beforeend',
        '<div class="pc-like"><div class="like-wrapper"><span><span class="pc-group"><span class="pc-dots"></span><span class="pc-dots"></span><span class="pc-dots"></span><span class="pc-dots"></span></span></span></div></div>'
      );
      prod_like.parentNode.querySelector('.pc-like').classList.add('pc-like-animate');
      // Remove like animation after 3 seconds
      setTimeout(function () {
        try {
          prod_like.parentNode.querySelector('.pc-like').remove();
        } catch (error) {}
      }, 3000);
    } else {
      // Remove like animation if checkbox is unchecked
      prod_like = event.target;
      try {
        prod_like.parentNode.querySelector('.pc-like').remove();
      } catch (error) {}
    }
  });
}

// Change authentication logo
var tc = document.querySelectorAll('.auth-main.v2 .img-brand');
for (var t = 0; t < tc.length; t++) {
  tc[t].setAttribute('src', '../assets/images/logo-white.svg');
}

// =======================================================
// =======================================================

var rtl_flag = false;
var dark_flag = false;

// Function to change layout dark/light settings
function layout_change_default() {
  // Check if dark mode is preferred and set layout accordingly
  if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    dark_layout = 'dark';
  } else {
    dark_layout = 'light';
  }
  layout_change(dark_layout); // Call layout_change function with dark_layout value

  // Set active state for default layout button
  var btn_control = document.querySelector('.theme-layout .btn[data-value="default"]');
  if (btn_control) {
    btn_control.classList.add('active');
  }

  // Event listener for dark mode preference change
  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (event) => {
    dark_layout = event.matches ? 'dark' : 'light';
    layout_change(dark_layout); // Call layout_change function with dark_layout value
  });
}

// This event listener executes when the DOM content is fully loaded
document.addEventListener('DOMContentLoaded', function () {
  // Check if elements with class 'preset-color' exist (switch preset-1 to preset-10 colors and change main colors according to preset-* value)
  var if_exist = document.querySelectorAll('.preset-color');
  if (if_exist) {
    // Iterate over preset color links and add click event listeners
    var preset_color = document.querySelectorAll('.preset-color > a');
    for (var h = 0; h < preset_color.length; h++) {
      var c = preset_color[h];
      c.addEventListener('click', function (event) {
        var targetElement = event.target;
        if (targetElement.tagName == 'SPAN') {
          targetElement = targetElement.parentNode;
        }
        // Extract the preset value and call preset_change function
        var presetValue = targetElement.getAttribute('data-value');
        preset_change(presetValue);
      });
    }
  }

  // Initialize SimpleBar on elements with class 'pct-body' for custom scrollbar
  if (document.querySelector('.pct-body')) {
    new SimpleBar(document.querySelector('.pct-body'));
  }

  // Reset layout on button click
  var layout_reset = document.querySelector('#layoutreset');
  if (layout_reset) {
    layout_reset.addEventListener('click', function (e) {
      location.reload();
      localStorage.setItem('layout', 'vertical');
    });
  }
});

// Functions to handle layout theme contrast change
function layout_theme_contrast_change(value) {
  // Set attribute based on value and update button state accordingly
  if (value == 'true') {
    document.getElementsByTagName('body')[0].setAttribute('data-pc-theme_contrast', 'true');
    var control = document.querySelector('.theme-contrast .btn.active');
    if (control) {
      document.querySelector('.theme-contrast .btn.active').classList.remove('active');
      document.querySelector(".theme-contrast .btn[data-value='true']").classList.add('active');
    }
  } else {
    document.getElementsByTagName('body')[0].setAttribute('data-pc-theme_contrast', 'false');
    var control = document.querySelector('.theme-contrast .btn.active');
    if (control) {
      document.querySelector('.theme-contrast .btn.active').classList.remove('active');
      document.querySelector(".theme-contrast .btn[data-value='false']").classList.add('active');
    }
  }
}

// Functions to handle layout caption change (caption hide/show in sidebar)
function layout_caption_change(value) {
  // Set attribute based on value and update button state accordingly
  if (value == 'true') {
    document.getElementsByTagName('body')[0].setAttribute('data-pc-sidebar-caption', 'true');
    var control = document.querySelector('.theme-nav-caption .btn.active');
    if (control) {
      document.querySelector('.theme-nav-caption .btn.active').classList.remove('active');
      document.querySelector(".theme-nav-caption .btn[data-value='true']").classList.add('active');
    }
  } else {
    document.getElementsByTagName('body')[0].setAttribute('data-pc-sidebar-caption', 'false');
    var control = document.querySelector('.theme-nav-caption .btn.active');
    if (control) {
      document.querySelector('.theme-nav-caption .btn.active').classList.remove('active');
      document.querySelector(".theme-nav-caption .btn[data-value='false']").classList.add('active');
    }
  }
}

// Functions to handle layout preset change (active class add/remove from preset-color according to click)
function preset_change(value) {
  // Set attribute based on value and update active preset color link
  document.getElementsByTagName('body')[0].setAttribute('data-pc-preset', value);
  var control = document.querySelector('.pct-offcanvas');
  if (control) {
    document.querySelector('.preset-color > a.active').classList.remove('active');
    document.querySelector(".preset-color > a[data-value='" + value + "']").classList.add('active');
  }
}

// Functions to handle main layout change (active class add/remove from theme-main-layout according to click)
function main_layout_change(value) {
  // Set attribute based on value and update active main layout link
  document.getElementsByTagName('body')[0].setAttribute('data-pc-layout', value);
  var control = document.querySelector('.pct-offcanvas');
  if (control) {
    var activeLink = document.querySelector('.theme-main-layout > a.active');
    if (activeLink) {
      activeLink.classList.remove('active');
    }
    var newActiveLink = document.querySelector(".theme-main-layout > a[data-value='" + value + "']");
    if (newActiveLink) {
      newActiveLink.classList.add('active');
    }
  }
}

// Function to handle layout direction change (LTR/RTL)
function layout_rtl_change(value) {
  // Set attribute based on value and update button state accordingly
  var control = document.querySelector('#layoutmodertl');
  if (value == 'true') {
    rtl_flag = true;
    document.getElementsByTagName('body')[0].setAttribute('data-pc-direction', 'rtl');
    document.getElementsByTagName('html')[0].setAttribute('dir', 'rtl');
    document.getElementsByTagName('html')[0].setAttribute('lang', 'ar');
    var control = document.querySelector('.theme-direction .btn.active');
    if (control) {
      document.querySelector('.theme-direction .btn.active').classList.remove('active');
      document.querySelector(".theme-direction .btn[data-value='true']").classList.add('active');
    }
  } else {
    rtl_flag = false;
    document.getElementsByTagName('body')[0].setAttribute('data-pc-direction', 'ltr');
    document.getElementsByTagName('html')[0].removeAttribute('dir');
    document.getElementsByTagName('html')[0].removeAttribute('lang');
    var control = document.querySelector('.theme-direction .btn.active');
    if (control) {
      document.querySelector('.theme-direction .btn.active').classList.remove('active');
      document.querySelector(".theme-direction .btn[data-value='false']").classList.add('active');
    }
  }
}

// Function to handle layout change (dark/light) and update related elements
function layout_change(layout) {
  // Set layout attribute and update related elements (e.g., logos)
  var control = document.querySelector('.pct-offcanvas');
  document.getElementsByTagName('body')[0].setAttribute('data-pc-theme', layout);

  var btn_control = document.querySelector('.theme-layout .btn[data-value="default"]');
  if (btn_control) {
    btn_control.classList.remove('active');
  }
  if (layout == 'dark') {
    dark_flag = true;
    if (document.querySelector('.pc-sidebar .m-header .logo-lg')) {
      document.querySelector('.pc-sidebar .m-header .logo-lg').setAttribute('src', '../assets/images/logo-white.svg');
    }

    if (document.querySelector('.navbar-brand .logo-lg')) {
      document.querySelector('.navbar-brand .logo-lg').setAttribute('src', '../assets/images/logo-white.svg');
    }
    if (document.querySelector('.auth-main.v1 .auth-sidefooter')) {
      document.querySelector('.auth-main.v1 .auth-sidefooter img').setAttribute('src', '../assets/images/logo-white.svg');
    }
    if (document.querySelector('.footer-top .footer-logo')) {
      document.querySelector('.footer-top .footer-logo').setAttribute('src', '../assets/images/logo-white.svg');
    }
    var control = document.querySelector('.theme-layout .btn.active');
    if (control) {
      document.querySelector('.theme-layout .btn.active').classList.remove('active');
      document.querySelector(".theme-layout .btn[data-value='false']").classList.add('active');
    }
  } else {
    dark_flag = false;
    if (document.querySelector('.pc-sidebar .m-header .logo-lg')) {
      document.querySelector('.pc-sidebar .m-header .logo-lg').setAttribute('src', '../assets/images/logo-dark.svg');
    }
    if (document.querySelector('.navbar-brand .logo-lg')) {
      document.querySelector('.navbar-brand .logo-lg').setAttribute('src', '../assets/images/logo-dark.svg');
    }
    if (document.querySelector('.auth-main.v1 .auth-sidefooter')) {
      document.querySelector('.auth-main.v1 .auth-sidefooter img').setAttribute('src', '../assets/images/logo-dark.svg');
    }
    if (document.querySelector('.footer-top .footer-logo')) {
      document.querySelector('.footer-top .footer-logo').setAttribute('src', '../assets/images/logo-dark.svg');
    }
    var control = document.querySelector('.theme-layout .btn.active');
    if (control) {
      document.querySelector('.theme-layout .btn.active').classList.remove('active');
      document.querySelector(".theme-layout .btn[data-value='true']").classList.add('active');
    }
  }
}

// Function to toggle box container class based on value (true/false)
function change_box_container(value) {
  if (document.querySelector('.pc-content')) {
    // Add or remove container class from specific elements based on value
    if (value == 'true') {
      document.querySelector('.pc-content').classList.add('container');
      document.querySelector('.footer-wrapper').classList.add('container');
      document.querySelector('.footer-wrapper').classList.remove('container-fluid');

      var control = document.querySelector('.theme-container .btn.active');
      if (control) {
        document.querySelector('.theme-container .btn.active').classList.remove('active');
        document.querySelector(".theme-container .btn[data-value='true']").classList.add('active');
      }
    } else {
      document.querySelector('.pc-content').classList.remove('container');
      var control = document.querySelector('.theme-container .btn.active');
      if (control) {
        document.querySelector('.theme-container .btn.active').classList.remove('active');
        document.querySelector(".theme-container .btn[data-value='false']").classList.add('active');
      }
    }
  }
}

// ----------    new setup end   ------------

// =======================================================
// =======================================================

// Function to remove CSS classes with a given prefix from a DOM node
function removeClassByPrefix(node, prefix) {
  // Iterate over class list and remove classes with the specified prefix
  for (let i = 0; i < node.classList.length; i++) {
    let value = node.classList[i];
    if (value.startsWith(prefix)) {
      node.classList.remove(value);
    }
  }
}

// Functions for sliding up, sliding down, and toggling visibility of elements
let slideUp = (target, duration = 0) => {
  // Slide up animation implementation
  target.style.transitionProperty = 'height, margin, padding';
  target.style.transitionDuration = duration + 'ms';
  target.style.boxSizing = 'border-box';
  target.style.height = target.offsetHeight + 'px';
  target.offsetHeight;
  target.style.overflow = 'hidden';
  target.style.height = 0;
  target.style.paddingTop = 0;
  target.style.paddingBottom = 0;
  target.style.marginTop = 0;
  target.style.marginBottom = 0;
};

let slideDown = (target, duration = 0) => {
  // Slide down animation implementation
  target.style.removeProperty('display');
  let display = window.getComputedStyle(target).display;

  if (display === 'none') display = 'block';

  target.style.display = display;
  let height = target.offsetHeight;
  target.style.overflow = 'hidden';
  target.style.height = 0;
  target.style.paddingTop = 0;
  target.style.paddingBottom = 0;
  target.style.marginTop = 0;
  target.style.marginBottom = 0;
  target.offsetHeight;
  target.style.boxSizing = 'border-box';
  target.style.transitionProperty = 'height, margin, padding';
  target.style.transitionDuration = duration + 'ms';
  target.style.height = height + 'px';
  target.style.removeProperty('padding-top');
  target.style.removeProperty('padding-bottom');
  target.style.removeProperty('margin-top');
  target.style.removeProperty('margin-bottom');
  window.setTimeout(() => {
    target.style.removeProperty('height');
    target.style.removeProperty('overflow');
    target.style.removeProperty('transition-duration');
    target.style.removeProperty('transition-property');
  }, duration);
};

var slideToggle = (target, duration = 0) => {
  // Slide toggle animation implementation
  if (window.getComputedStyle(target).display === 'none') {
    return slideDown(target, duration);
  } else {
    return slideUp(target, duration);
  }
};

// =======================================================
// =======================================================
